using UnityEngine;

public class CarAgent
{
    public CarData carData;
    public NeuralNetwork network;
    private float fitnessScore = 0f;

    public CarAgent(CarData config, NeuralNetwork net)
    {
        carData = config;
        network = net;
    }

    public void UpdateFitness(float distanceTravelled, Rigidbody rb)
    {
        float forwardProgress = Vector3.Dot(rb.velocity.normalized, rb.transform.forward);
        fitnessScore = distanceTravelled * Mathf.Max(0, forwardProgress);
    }

    public float GetFitness()
    {
        return fitnessScore;
    }
}