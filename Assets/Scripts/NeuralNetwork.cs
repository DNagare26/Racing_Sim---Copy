using System;
using UnityEngine;

/// <summary>
/// Represents a fully connected feedforward neural network for AI decision-making.
/// </summary>
public class NeuralNetwork
{
    public int[] layers; 
    public float[][] neurons; 
    public float[][][] weights; 

    public NeuralNetwork(int[] layers)
    {
        this.layers = layers;
        InitialiseNeurons();
        InitialiseWeights();
    }

    private void InitialiseNeurons()
    {
        neurons = new float[layers.Length][];
        for (int i = 0; i < layers.Length; i++)
        {
            neurons[i] = new float[layers[i]];
        }
    }

    private void InitialiseWeights()
    {
        weights = new float[layers.Length - 1][][];
        for (int i = 0; i < weights.Length; i++)
        {
            weights[i] = new float[layers[i]][];
            for (int j = 0; j < layers[i]; j++)
            {
                weights[i][j] = new float[layers[i + 1]];
                for (int k = 0; k < layers[i + 1]; k++)
                {
                    weights[i][j][k] = UnityEngine.Random.Range(-1f, 1f);
                }
            }
        }
    }

    public float[] ForwardPass(float[] inputs)
    {
        if (inputs.Length != neurons[0].Length)
        {
            throw new ArgumentException("Input size does not match network input layer.");
        }

        neurons[0] = inputs;

        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < layers[i]; j++)
            {
                double value = 0f;
                for (int k = 0; k < layers[i - 1]; k++)
                {
                    value += neurons[i - 1][k] * weights[i - 1][k][j];
                }

                neurons[i][j] = (float)Math.Tanh(value);
            }
        }

        return neurons[layers.Length - 1];
    }

    public NeuralNetwork Mutate(float mutationRate = 0.1f)
    {
        NeuralNetwork mutatedNetwork = new NeuralNetwork(layers);

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    if (UnityEngine.Random.value < mutationRate)
                    {
                        mutatedNetwork.weights[i][j][k] = weights[i][j][k] + UnityEngine.Random.Range(-0.2f, 0.2f);
                    }
                    else
                    {
                        mutatedNetwork.weights[i][j][k] = weights[i][j][k];
                    }
                }
            }
        }

        return mutatedNetwork;
    }
}
