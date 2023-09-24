using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;

public class GeneticAlgorithmManager : MonoBehaviour
{
    [Header("References")]
    public NetworkCarController controller;

    [Header("Controls")]
    public int initialPopulation = 85;
    [Range(0.0f, 1.0f)]
    public float mutationRate = 0.055f;

    [Header("Crossover Controls")]
    public int bestAgentSelection = 8;
    public int worstAgentSelection = 0;
    public int numberToCrossover;

    private List<int> genePool = new List<int>();

    private int naturallySelected;

    private NeuralNetwork[] population;

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome = 0;

    private void Start()
    {
        CreatePopulation();
    }

    private void CreatePopulation() 
    {
        population = new NeuralNetwork[initialPopulation];

        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }

    private void ResetToCurrentGenome()
    {
        controller.ResetWithNetwork(population[currentGenome]);
    }

    private void FillPopulationWithRandomValues (NeuralNetwork[] newPopulation, int startingIndex)
    {
        while (startingIndex < initialPopulation)
        {
            newPopulation[startingIndex] = new NeuralNetwork();
            newPopulation[startingIndex].Initialise(controller.layers, controller.neurons);
            startingIndex ++;
        }
    }

    public void Death(float fitness, NeuralNetwork network)
    {
        if (currentGenome < population.Length - 1)
        {
            population[currentGenome].fitness = fitness;
            currentGenome ++;
            ResetToCurrentGenome();
        }
        else
        {
            Repopulate();
        }
    }

    private void Repopulate()
    {
        genePool.Clear();
        currentGeneration ++;
        naturallySelected = 0;

        SortPopulation();

        NeuralNetwork[] newPopulation = PickBestPopulation();

        Crossover(newPopulation);
        Mutate(newPopulation);

        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        population = newPopulation;
        currentGenome = 0;

        ResetToCurrentGenome();
    }

    private void Crossover(NeuralNetwork[] newPopulation)
    {
        // Crossover function produces 2 children, so double increment used
        for (int i = 0; i < numberToCrossover; i += 2)
        {
            int AIndex = i;
            int BIndex = i + 1;

            if (genePool.Count >= 1)
            {
                for (int j = 0; j < 100; j++)
                {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];

                    if (AIndex != BIndex)
                    {
                        break;
                    }
                }
            }

            NeuralNetwork Child1 = new NeuralNetwork();
            NeuralNetwork Child2 = new NeuralNetwork();

            Child1.Initialise(controller.layers, controller.neurons);
            Child2.Initialise(controller.layers, controller.neurons);

            Child1.fitness = 0;
            Child2.fitness = 0;

            // Crossover Weights
            for (int j = 0; j < Child1.weights.Count; j++)
            {
                // 50-50 chance
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.weights[j] = population[AIndex].weights[j];
                    Child2.weights[j] = population[BIndex].weights[j];
                }
                else
                {
                    Child1.weights[j] = population[BIndex].weights[j];
                    Child2.weights[j] = population[AIndex].weights[j];
                }
            }

            // Crossover Biases
            for (int j = 0; j < Child1.biases.Count; j++)
            {
                // 50-50 chance
                if (Random.Range(0.0f, 1.0f) < 0.5f)
                {
                    Child1.biases[j] = population[AIndex].biases[j];
                    Child2.biases[j] = population[BIndex].biases[j];
                }
                else
                {
                    Child1.biases[j] = population[BIndex].biases[j];
                    Child2.biases[j] = population[AIndex].biases[j];
                }
            }

            // Post-increments the naturallySelected counter
            newPopulation[naturallySelected++] = Child1;

            newPopulation[naturallySelected++] = Child2;
        }
    }

    private void Mutate(NeuralNetwork[] newPopulation)
    {
        for (int i = 0; i < naturallySelected; i++)
        {
            for (int j = 0; j < newPopulation[i].weights.Count; j++)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[j] = MutateMatrix(newPopulation[i].weights[j]);
                }
            }
        }
    }

    Matrix<float> MutateMatrix (Matrix<float> inMatrix)
    {
        // Caps mutatable values to 1/7th the size of the matrix
        int randomPoints = Random.Range(1, inMatrix.RowCount * inMatrix.ColumnCount / 7);

        Matrix<float> temp = inMatrix;

        for (int i = 0; i < randomPoints; i++)
        {
            int randomColumn = Random.Range(0, temp.ColumnCount);
            int randomRow = Random.Range(0, temp.RowCount);

            temp[randomRow, randomColumn] = Mathf.Clamp(temp[randomRow, randomColumn] + Random.Range(-0.2f, 0.2f), -1f, 1f);
        }

        return temp;
    }

    private void SortPopulation()
    {
        List<NeuralNetwork> temp = population.ToList();
        temp.Sort((x,y) => y.fitness.CompareTo(x.fitness));
        population = temp.ToArray();
    }

    private NeuralNetwork[] PickBestPopulation()
    {
        NeuralNetwork[] newPopulation = new NeuralNetwork[initialPopulation];

        for (int i = 0; i < bestAgentSelection; i++, naturallySelected++)
        {
            newPopulation[naturallySelected] = population[i].InitialiseCopy(controller.layers, controller.neurons);
            newPopulation[naturallySelected].fitness = 0;

            // Weights number of additions to the fitness of the algorithm
            int f = Mathf.RoundToInt(population[i].fitness * 10);

            for (int j = 0; j < f; j++)
            {
                // Indexes used to reference, minimises time spent on slow copy algorithm
                genePool.Add(i);
            }
        }

        // Going through the worst maintains population diversity
        for (int i = 0; i < worstAgentSelection; i++)
        {
            int last = population.Length - 1 - i;

            // Weights number of additions to the fitness of the algorithm
            int f = Mathf.RoundToInt(population[last].fitness * 10);

            for (int j = 0; j < f; j++)
            {
                // Indexes used to reference, minimises time spent on slow copy algorithm
                genePool.Add(last);
            }
        }

        return newPopulation;
    }

}
