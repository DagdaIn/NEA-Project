using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Network_Layer
{
    #region Variables
    public float[,] weights;
    public float[] biases;
    public int numNodes;
    public int numOutNodes;
    #endregion

    #region Constructors
    /// <summary>
    /// Assigns variables and gives weights and biases default values
    /// </summary>
    /// <param name="inNumNodes">The number of nodes in this layer</param>
    /// <param name="inNumOutNodes">The number of nodes in the next layer</param>
    public Network_Layer(int inNumNodes, int inNumOutNodes)
    {
        this.numNodes = inNumNodes;
        this.numOutNodes = inNumOutNodes;

        this.weights = new float[this.numNodes, this.numOutNodes];
        this.biases = new float[this.numOutNodes];
    }
    #endregion

    #region Methods
    /// <summary>
    /// Sets all weights and biases to a random value between minValue and maxValue in this layer
    /// </summary>
    /// <param name="minValue">The minimum value that a weight or bias can be</param>
    /// <param name="maxValue">The maximum value that a weight or bias can be</param>
    public void SetRandomWeightsAndBiases(float minValue, float maxValue)
    {
        System.Random rnd = new System.Random();
        for (int i = 0; i < this.weights.GetLength(0); i++)
        {
            for (int j = 0; j < this.weights.GetLength(1); j++)
            {
                this.weights[i,j] = minValue + (float)rnd.NextDouble() * Math.Abs(maxValue - minValue);
            }
        }

        for (int i = 0; i < this.biases.Length; i++)
        {
            this.biases[i] = minValue + (float)rnd.NextDouble() * Math.Abs(maxValue - minValue);
        }
    }

    /// <summary>
    /// Passes a set of inputs through this layer
    /// </summary>
    /// <param name="inputs">The value of the nodes in this layer</param>
    /// <returns>An array of values representing the output of this layer</returns>
    public float[] CalculateOutputs(float[] inputs)
    {
        float[] outputs = new float[this.numOutNodes];

        for (int i = 0; i < inputs.Length; i++)
        {
            for (int j = 0; j < outputs.Length; j++)
            {
                outputs[j] += inputs[i] * weights[i,j];
            }
        }

        for (int i = 0; i < outputs.Length; i++)
        {
            outputs[i] += biases[i];
            outputs[i] = ActivationFunction(outputs[i]);
        }

        return outputs;
    }

    /// <summary>
    /// Activation function for any given node
    /// </summary>
    /// <param name="value">The raw value</param>
    /// <returns>The value after the activation function</returns>
    public float ActivationFunction(float value)
    {
        // return 1f / (1f + (float)Math.Exp(-value)); // Sigmoid
        return (float)Math.Tanh(value); // Tanh
    }

    /// <summary>
    /// Copies this layer of the network
    /// </summary>
    /// <returns>An identical copy of this layer</returns>
    public Network_Layer copy()
    {
        Network_Layer newLayer = new Network_Layer(this.numNodes, this.numOutNodes);

        for (int i = 0; i < this.weights.GetLength(0); i++)
        {
            for (int j = 0; j < this.weights.GetLength(1); j++)
            {
                newLayer.weights[i,j] = this.weights[i,j];
            }
        }

        for (int i = 0; i < this.biases.Length; i++)
        {
            newLayer.biases[i] = this.biases[i];
        }

        return newLayer;
    }
    #endregion

}
