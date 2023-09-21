using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

public class Neural_Network
{
    #region Variables

    public int[] layerNumNodes;
    public Network_Layer[] layers;
    public float fitness;
    public float[] Genome
    {
        get => getGenome();
        set => setGenome(value);
    }

    #endregion
    
    #region Constructors
    /// <summary>
    /// Sets variables to their default values
    /// </summary>
    /// <param name="inLayerNumNodes">Number of nodes in each layer</param>
    public Neural_Network(int[] inLayerNumNodes)
    {
        this.fitness = 0f;
        layers = new Network_Layer[inLayerNumNodes.Length - 1];
        layerNumNodes = inLayerNumNodes;

        for (int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Network_Layer(layerNumNodes[i], layerNumNodes[i + 1]);
        }

        InitLayers();
    }
    #endregion

    #region Properties
    /// <summary>
    /// Retrieves the genome from each layer
    /// </summary>
    /// <returns>The networks weights and biases as one array</returns>
    private float[] getGenome()
    {
        List<float> tempGenome = new List<float>();
        for (int i = 0; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i].weights.GetLength(0); j++)
            {
                for (int k = 0; k < layers[i].weights.GetLength(1); k++)
                {
                    tempGenome.Add(layers[i].weights[j,k]);
                }
            }

            for (int j = 0; j < layers[i].biases.Length; j++)
            {
                tempGenome.Add(layers[i].biases[j]);
            }
        }

        return tempGenome.ToArray();
    }

    /// <summary>
    /// Sets the genome to each layer
    /// </summary>
    /// <param name="value">The new genome</param>
    private void setGenome(float[] value)
    {
        int count = 0;
        for (int i = 0; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i].weights.GetLength(0); j++)
            {
                for (int k = 0; k < layers[i].weights.GetLength(1); k++)
                {
                    layers[i].weights[j,k] = value[count];
                    count++;
                }
            }

            for (int j = 0; j < layers[i].biases.Length; j++)
            {
                layers[i].biases[j] = value[count];
                count++;
            }
        }
    }

    #endregion

    #region Methods
    /// <summary>
    /// Feeds a set of inputs through the network
    /// </summary>
    /// <param name="inputs">The inputs to the network</param>
    /// <returns>The values returned after feeding the inputs through the network</returns>
    public float[] CalculateOutputs(float[] inputs)
    {
        for (int i = 0; i < layers.Length; i++)
        {
            inputs = layers[i].CalculateOutputs(inputs);
        }

        return inputs;
    }

    public void InitLayers()
    {
        foreach (Network_Layer layer in layers)
        {
            layer.SetRandomWeightsAndBiases(-0.5f, 0.5f);
        }
    }

    /// <summary>
    /// Copies this network to a new network
    /// </summary>
    /// <returns>An identical network</returns>
    public Neural_Network copy()
    {
        Neural_Network newNetwork = new Neural_Network(this.layerNumNodes);
        for (int i = 0; i < layers.Length; i++)
        {
            newNetwork.layers[i] = this.layers[i].copy();
        }

        newNetwork.fitness = this.fitness;

        return newNetwork;
    }
    #endregion
}
