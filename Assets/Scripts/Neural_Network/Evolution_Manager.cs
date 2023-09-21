using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Evolution_Manager
{
    #region Variables
    public Neural_Network[] networks;
    private int[] networkTopology;
    #endregion

    #region Constructors
    public Evolution_Manager()
    {
        networkTopology = new int[] {5, 4, 3, 2};
        networks = new Neural_Network[100];
        for (int i = 0; i < 100; i++)
        {
            networks[i] = new Neural_Network(networkTopology);
        }
    }
    #endregion

    #region Methods
    public Neural_Network[] NextGeneration()
    {
        Neural_Network[] elites = new Neural_Network[2];

        for (int i = 0; i < networks.Length; i++)
        {
            networks[i] = networks[i].copy();
        }

        elites[0] = networks[0].copy();
        elites[1] = networks[1].copy();

        for (int i = 2; i < networks.Length; i++)
        {
            if (elites[0].fitness < networks[i].fitness && elites[1].fitness >= networks[i].fitness)
            {
                elites[0] = networks[i].copy();
            }
            else if (elites[1].fitness < networks[i].fitness && elites[0].fitness >= networks[i].fitness)
            {
                elites[1] = networks[i].copy();
            }
        }

        Neural_Network[] crossover = new Neural_Network[2];
        CrossoverNetworks(elites[0], elites[1], out crossover[0], out crossover[1]);

        networks[0] = elites[0].copy();
        networks[1] = elites[1].copy();

        for (int i = 2; i < networks.Length; i++)
        {
            MutateNetwork(crossover[i % 2], out networks[i]);
        }
        return networks;
    }

    private void MutateNetwork(Neural_Network parent, out Neural_Network child)
    {
        float[] Genome = parent.Genome;

        float mutationChance = 0.3f;
        float maxMutation = 0.5f;

        for (int i = 0; i < Genome.Length; i++)
        {
            if (Random.Range(0f, 1f) < mutationChance)
            {
                Genome[i] = Genome[i] + Random.Range(-maxMutation, maxMutation);
            }
        }

        child = new Neural_Network(parent.layerNumNodes);
        child.Genome = Genome;
    }

    private void CrossoverNetworks(Neural_Network parent1, Neural_Network parent2, out Neural_Network child1, out Neural_Network child2)
    {
        float[] parent1Genome = parent1.Genome;
        float[] parent2Genome = parent2.Genome;
        float[] child1Genome = new float[parent1Genome.Length];
        float[] child2Genome = new float[parent2Genome.Length];

        for (int i = 0; i < parent1Genome.Length; i++)
        {
            if (Random.Range(0f, 1f) < 0.2f)
            {
                child1Genome[i] = parent2Genome[i];
                child2Genome[i] = parent1Genome[i];
            }
            else
            {
                child1Genome[i] = parent1Genome[i];
                child2Genome[i] = parent2Genome[i];
            }
        }

        child1 = new Neural_Network(parent1.layerNumNodes);
        child2 = new Neural_Network(parent2.layerNumNodes);
        child1.Genome = child1Genome;
        child2.Genome = child2Genome;
    }
    #endregion
}
