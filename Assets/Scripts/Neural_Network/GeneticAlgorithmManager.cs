#region includes
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Unity.Collections.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
#endregion

public class GeneticAlgorithmManager : MonoBehaviour
{
    #region Public Variables
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

    [Header("Public View")]
    public int currentGeneration;
    public int currentGenome = 0;
    #endregion

    #region Private Variables
    private List<int> genePool = new List<int>();

    private int naturallySelected;

    private NeuralNetwork[] population;
    #endregion

    #region Initial Population Management
    /// <summary>
    /// Before the first frame, the generation is created
    /// </summary>
    private void Start()
    {
        CreatePopulation();
    }

    /// <summary>
    /// Defines population as an array of neural networks, randomises all networks,
    /// and sets the current genome to the first network in the array.
    /// </summary>
    private void CreatePopulation() 
    {
        population = new NeuralNetwork[initialPopulation];

        FillPopulationWithRandomValues(population, 0);
        ResetToCurrentGenome();
    }
    #endregion

    #region Vehicle management
    /// <summary>
    /// Resets the vehicle, and sets the network to the current genome
    /// </summary>
    private void ResetToCurrentGenome()
    {
        controller.ResetWithNetwork(population[currentGenome]);
    }

    /// <summary>
    /// Moves on to the network after the current network
    /// </summary>
    /// <param name="fitness"></param>
    /// <param name="network"></param>
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
    #endregion

    #region Generation Creation
    /// <summary>
    /// Fills each network, starting at <paramref name="startingIndex"/> of
    /// <paramref name="newPopulation"/> with random values
    /// </summary>
    /// <param name="newPopulation">Where the new population should be stored</param>
    /// <param name="startingIndex">The first network to be randomised</param>
    private void FillPopulationWithRandomValues (NeuralNetwork[] newPopulation, int startingIndex)
    {
        while (startingIndex < initialPopulation)
        {
            newPopulation[startingIndex] = new NeuralNetwork();
            newPopulation[startingIndex].Initialise(controller.layers, controller.neurons);
            startingIndex ++;
        }
    }

    /// <summary>
    /// Creates the next generation of networks
    /// </summary>
    private void Repopulate()
    {
        // Iterates to the next generation
        genePool.Clear();
        currentGeneration ++;
        naturallySelected = 0;

        // Ensures the population has the most fit at the start, and worst at the back
        SortPopulation();

        // Picks the networks to use to breed the next generation
        NeuralNetwork[] newPopulation = PickBestPopulation();

        // Creates new networks to fill next generation with
        Crossover(newPopulation);
        Mutate(newPopulation);

        // Each generation has a certain amount of fully random networks
        FillPopulationWithRandomValues(newPopulation, naturallySelected);

        // Resets the population to the new population
        population = newPopulation;
        currentGenome = 0;

        ResetToCurrentGenome();
    }
    #endregion

    #region Evolution Methods
    /// <summary>
    /// Combines every 2 networks in <paramref name="newPopulation"/>, e.g. 0 and 1, 2 and 3, etc.
    /// </summary>
    /// <param name="newPopulation">The population after the crossover procedure</param>
    private void Crossover(NeuralNetwork[] newPopulation)
    {
        // Crossover function produces 2 children, so double increment used
        for (int i = 0; i < numberToCrossover; i += 2)
        {
            int AIndex = i;
            int BIndex = i + 1;

            int counter = 0;

            // Checks that there are enough networks to crossover
            if (genePool.Count > 1)
            {
                // Randomly creates two discrete values
                do
                {
                    AIndex = genePool[Random.Range(0, genePool.Count)];
                    BIndex = genePool[Random.Range(0, genePool.Count)];
                    counter ++;

                // Sets a hard limit of 100 on the number of random 'guesses'
                } while(AIndex == BIndex || counter < 100);
            }

            // Instantiates two children
            NeuralNetwork Child1 = new NeuralNetwork();
            NeuralNetwork Child2 = new NeuralNetwork();

            // Initialises the children so they are the same size as their parents
            Child1.Initialise(controller.layers, controller.neurons);
            Child2.Initialise(controller.layers, controller.neurons);

            // Sets their fitness to be 0
            Child1.fitness = 0;
            Child2.fitness = 0;

            // Crossover Weights
            for (int j = 0; j < Child1.weights.Count; j++)
            {
                // 50-50 chance of crossover
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
                // 50-50 chance of crossover
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

    /// <summary>
    /// Randomly changes a value within the network to another valid value
    /// </summary>
    /// <param name="newPopulation">The new population of networks</param>
    private void Mutate(NeuralNetwork[] newPopulation)
    {
        // Iterates through all networks that should be mutated
        for (int i = 0; i < naturallySelected; i++)
        {
            // Randomly mutates the weights
            for (int j = 0; j < newPopulation[i].weights.Count; j++)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].weights[j] = MutateMatrix(newPopulation[i].weights[j]);
                }
            }

            // Randomly mutates the biases
            for (int j = 0; j < newPopulation[i].biases.Count; j++)
            {
                if (Random.Range(0.0f, 1.0f) < mutationRate)
                {
                    newPopulation[i].biases[j] = Mathf.Clamp(newPopulation[i].biases[j] + Random.Range(-0.2f, 0.2f), -1f, 1f);
                }
            }
        }
    }

    /// <summary>
    /// Mutates all the values within a matrix 
    /// </summary>
    /// <param name="inMatrix">The matrix to be mutated</param>
    /// <returns>A new matrix with mutated values</returns>
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

    /// <summary>
    /// Selects the networks from the current population that should produce the next generation
    /// </summary>
    /// <returns>An array of the best networks</returns>
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
    #endregion

    #region Sorting
    /// <summary>
    /// Sorts a list of neural networks by fitness descending
    /// </summary>
    private void SortPopulation()
    {
        List<NeuralNetwork> temp = population.ToList();
        temp.Sort((x,y) => y.fitness.CompareTo(x.fitness));
        population = temp.ToArray();
    }
    #endregion
}
