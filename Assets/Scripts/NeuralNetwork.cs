using System;
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
        InitialiseWeights();
    }

    private void InitialiseWeights()
    {
        for (int i = 0; i < inputNodes; i++)
            for (int j = 0; j < hiddenNodes; j++)
                weightsInputHidden[i, j] = RandomRange(-1f, 1f);

        for (int i = 0; i < hiddenNodes; i++)
            for (int j = 0; j < outputNodes; j++)
                weightsHiddenOutput[i, j] = RandomRange(-1f, 1f);
    }

    private float[] ActivateLayer(float[] inputs, float[,] weights)
    {
        int outputSize = weights.GetLength(1);
        float[] outputs = new float[outputSize];

        for (int j = 0; j < outputSize; j++)
        {
            float sum = 1.0f; // Bias term
            for (int i = 0; i < weights.GetLength(0); i++)
                sum += inputs[i] * weights[i, j];

            outputs[j] = System.MathF.Tanh(sum); // Activation function
        }
        return outputs;
    }

    public float[] ForwardPass(float[] inputs)
    {
        float[] hiddenOutputs = ActivateLayer(inputs, weightsInputHidden);
        return ActivateLayer(hiddenOutputs, weightsHiddenOutput);
    }

    public NeuralNetwork Mutate(float generationCount)
    {
        NeuralNetwork mutatedNetwork = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
        float mutationStrength = Mathf.Lerp(0.5f, 0.1f, generationCount / 50f);
        float mutationProbability = 0.1f;

        for (int i = 0; i < inputNodes; i++)
        {
            for (int j = 0; j < hiddenNodes; j++)
            {
                mutatedNetwork.weightsInputHidden[i, j] = weightsInputHidden[i, j];
                if (UnityEngine.Random.value < mutationProbability)
                {
                    mutatedNetwork.weightsInputHidden[i, j] += RandomRange(-mutationStrength, mutationStrength);
                }
            }
        }

        for (int i = 0; i < hiddenNodes; i++)
        {
            for (int j = 0; j < outputNodes; j++)
            {
                mutatedNetwork.weightsHiddenOutput[i, j] = weightsHiddenOutput[i, j];
                if (UnityEngine.Random.value < mutationProbability)
                {
                    mutatedNetwork.weightsHiddenOutput[i, j] += RandomRange(-mutationStrength, mutationStrength);
                }
            }
        }

        return mutatedNetwork;
    }

    private float RandomRange(float min, float max)
    {
        return UnityEngine.Random.Range(min, max);
    }
}
