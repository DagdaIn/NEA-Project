#region includes
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using Random = UnityEngine.Random;
#endregion

#region Save Data
[System.Serializable]
public struct NetworkData
{
    public void SetData(NeuralNetwork network)
    {
        this.inputLayer = network.inputLayer;
        this.hiddenLayers = network.hiddenLayers;
        this.outputLayer = network.outputLayer;
        this.weights = network.weights;
        this.biases = network.biases;
    }

    public Matrix<float> inputLayer;
    public List<Matrix<float>> hiddenLayers;
    public Matrix<float> outputLayer;
    public List<Matrix<float>> weights;
    public List<float> biases;
}
#endregion

public class NeuralNetwork
{
    #region Public Variables
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 5);

    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

    public List<Matrix<float>> weights = new List<Matrix<float>>();

    public List<float> biases = new List<float>();
    public float fitness;
    #endregion

    #region Saving
    /// <summary>
    /// Saves the network to "NetworkData.json" 
    /// -- To be implemented -- 
    /// </summary>
    public void SaveNetwork()
    {
        NetworkData data = new NetworkData();
        data.SetData(this);
        string stringData = JsonUtility.ToJson(data);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/NetworkData.json", stringData);
    }

    /// <summary>
    /// Loads the network stored in "NetworkData.json"
    /// -- To be implemented --
    /// </summary>
    public void LoadNetwork()
    {
        string stringData;
        using (StreamReader sr = new StreamReader("NetworkData.json"))
        {
            stringData = sr.ReadToEnd();
        }

        NetworkData data = new NetworkData();
        data = JsonUtility.FromJson<NetworkData>(stringData);
    }
    #endregion

    #region Initialisation
    /// <summary>
    /// Creates a copy of this network
    /// </summary>
    /// <param name="hiddenLayerCount">The number of hidden layers</param>
    /// <param name="hiddenNeuronCount">The number of nodes in each hidden layer</param>
    /// <returns>A neural network identical to this one</returns>
    public NeuralNetwork InitialiseCopy(int hiddenLayerCount, int hiddenNeuronCount)
    {
        NeuralNetwork newNet = new NeuralNetwork();

        List<Matrix<float>> newWeights = new List<Matrix<float>>();

        for (int i = 0; i < this.weights.Count; i++)
        {
            Matrix<float> currentWeight = Matrix<float>.Build.Dense(weights[i].RowCount, weights[i].ColumnCount);

            for (int j = 0; j < currentWeight.RowCount; j++)
            {
                for (int k = 0; k < currentWeight.ColumnCount; k++)
                {
                    currentWeight[j,k] = weights[i][j, k];
                }
            }

            newWeights.Add(currentWeight);
        }

        List<float> newBiases = new List<float>();

        newBiases.AddRange(biases);

        newNet.biases = newBiases;
        newNet.weights = newWeights;

        newNet.InitialiseHidden(hiddenLayerCount, hiddenNeuronCount);

        return newNet;
    }

    /// <summary>
    /// Sets up the hidden layers of the network
    /// </summary>
    /// <param name="hiddenLayerCount">The number of hidden layers</param>
    /// <param name="hiddenNeuronCount">The number of neurons per hidden layer</param>
    public void InitialiseHidden(int hiddenLayerCount, int hiddenNeuronCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {
            Matrix<float> newHiddenLayer = Matrix<float>.Build.Dense(1, hiddenLayerCount);
            hiddenLayers.Add(newHiddenLayer);
        }
    }
    
        /// <summary>
    /// Sets all values of the network to their defaults
    /// </summary>
    /// <param name="hiddenLayerCount">The number of hidden layers</param>
    /// <param name="hiddenNeuronCount">The number of neurons per hidden layer</param>
    public void Initialise(int hiddenLayerCount, int hiddenNeuronCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hiddenLayerCount + 1; i++)
        {
            Matrix<float> f = Matrix<float>.Build.Dense(1, hiddenNeuronCount);

            hiddenLayers.Add(f);

            biases.Add(Random.Range(-1f, 1f));

            if (i == 0)
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(5, hiddenNeuronCount);
                weights.Add(inputToH1);
            }

            Matrix<float> HiddenToHidden = Matrix<float>.Build.Dense(hiddenNeuronCount, hiddenNeuronCount);
            weights.Add(HiddenToHidden);
        }

        Matrix<float> OutputWeight = Matrix<float>.Build.Dense(hiddenNeuronCount, 2);
        weights.Add(OutputWeight);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();
    }
    #endregion

    /// <summary>
    /// Randomly assigns the weights of this network
    /// </summary>
    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int j = 0; j < weights[i].RowCount; j++)
            {
                for (int k = 0; k < weights[i].ColumnCount; k++)
                {
                    weights[i][j,k] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    #region Forward Propagation
    /// <summary>
    /// Takes in a float array as an input, and runs the network
    /// </summary>
    /// <param name="inputs">The inputs to the first layer of the network</param>
    /// <returns>Two floats representing the outputs of the network</returns>
    public (float, float) RunNetwork (float[] inputs)
    {
        for (int i = 0; i < 5; i++)
        {
            inputLayer[0, i] = inputs[i];
        }

        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count-1] * weights[weights.Count-1]) + biases[biases.Count-1]).PointwiseTanh();

        // Acceleration, turning
        return (Sigmoid(outputLayer[0, 0]), (float)Math.Tanh(outputLayer[0, 1]));
    }

    /// <summary>
    /// The sigmoid activation function
    /// </summary>
    /// <param name="input">The value to be activated</param>
    /// <returns>The activated value (between -1 and 1)</returns>
    private float Sigmoid (float input)
    {
        return (1/(1 + Mathf.Exp(-input)));
    }
    #endregion
}
