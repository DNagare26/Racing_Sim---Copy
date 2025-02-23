using System;
using System.Linq;
using UnityEngine;

public class NeuralNetwork
{
    private int inputNodes;
    private int hiddenNodes;
    private int outputNodes;
    private float[,] weightsInputHidden;
    private float[,] weightsHiddenOutput;

    public NeuralNetwork(int input, int hidden, int output)
    {
        inputNodes = input;
        hiddenNodes = hidden;
        outputNodes = output;
        weightsInputHidden = new float[inputNodes, hiddenNodes];
        weightsHiddenOutput = new float[hiddenNodes, outputNodes];
        InitializeWeights();
    }

    private void InitializeWeights()
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < inputNodes; i++)
            for (int j = 0; j < hiddenNodes; j++)
                weightsInputHidden[i, j] = (float)(rand.NextDouble() * 2 - 1);

        for (int i = 0; i < hiddenNodes; i++)
            for (int j = 0; j < outputNodes; j++)
                weightsHiddenOutput[i, j] = (float)(rand.NextDouble() * 2 - 1);
    }

    private float[] ActivateLayer(float[] inputs, float[,] weights)
    {
        float[] outputs = new float[weights.GetLength(1)];
        for (int j = 0; j < weights.GetLength(1); j++)
        {
            float sum = 0;
            for (int i = 0; i < weights.GetLength(0); i++)
                sum += inputs[i] * weights[i, j];
            outputs[j] = (float)Math.Tanh(sum);
        }
        return outputs;
    }

    public float[] ForwardPass(float[] inputs) // Renamed from FeedForward()
    {
        float[] hiddenOutputs = ActivateLayer(inputs, weightsInputHidden);
        return ActivateLayer(hiddenOutputs, weightsHiddenOutput);
    }

    public NeuralNetwork Mutate(float mutationRate) // Modified to return a new network
    {
        NeuralNetwork mutatedNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);

        System.Random rand = new System.Random();
        
        for (int i = 0; i < inputNodes; i++)
            for (int j = 0; j < hiddenNodes; j++)
            {
                mutatedNetwork.weightsInputHidden[i, j] = weightsInputHidden[i, j];
                if (rand.NextDouble() < mutationRate)
                    mutatedNetwork.weightsInputHidden[i, j] += (float)(rand.NextDouble() * 2 - 1) * 0.1f;
            }

        for (int i = 0; i < hiddenNodes; i++)
            for (int j = 0; j < outputNodes; j++)
            {
                mutatedNetwork.weightsHiddenOutput[i, j] = weightsHiddenOutput[i, j];
                if (rand.NextDouble() < mutationRate)
                    mutatedNetwork.weightsHiddenOutput[i, j] += (float)(rand.NextDouble() * 2 - 1) * 0.1f;
            }

        return mutatedNetwork;
    }
}
