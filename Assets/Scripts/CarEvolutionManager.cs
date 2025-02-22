using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles AI evolution using 1+1 Evolution Strategy with 10 AI cars.
/// </summary>
public class CarEvolutionManager : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform[] spawnPoints; // Array of 10 spawn points
    public int totalGenerations = 100;
    public float trainingTime = 120f;

    private List<CarController> cars = new List<CarController>();
    private List<NeuralNetwork> networks = new List<NeuralNetwork>();
    private int generationCount = 1;
    private bool isTraining = true;
    private float mutationRate = 0.1f; // Initial mutation rate

    private void Start()
    {
        InitialiseNetworks();
        StartCoroutine(TrainingLoop());
    }

    private void InitialiseNetworks()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            networks.Add(new NeuralNetwork(new int[] { 5, 8, 4 }));
        }
    }

    private IEnumerator TrainingLoop()
    {
        while (isTraining && generationCount <= totalGenerations)
        {
            SpawnCars();
            yield return new WaitForSeconds(trainingTime);
            EvaluateAndEvolve();
            generationCount++;
        }

        Debug.Log("Training Complete! Loading Race Scene...");
        GameManager.instance.SetFinalAIModels(networks);
        UnityEngine.SceneManagement.SceneManager.LoadScene("RaceScene");
    }

    private void SpawnCars()
    {
        foreach (var car in cars) Destroy(car.gameObject);
        cars.Clear();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            GameObject carObj = Instantiate(carPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            CarController car = carObj.GetComponent<CarController>();
            car.SetNeuralNetwork(networks[i]);
            cars.Add(car);
        }
    }

    private float lastBestFitness = 0f;

    private void EvaluateAndEvolve()
    {
        List<CarController> sortedCars = new List<CarController>(cars);
        sortedCars.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));

        float bestFitness = sortedCars[0].GetFitness();
    
        if (bestFitness > lastBestFitness)
        {
            mutationRate *= 0.9f; // Reduce mutation if progress is made
        }
        else
        {
            mutationRate *= 1.1f; // Increase mutation if stuck
        }

        mutationRate = Mathf.Clamp(mutationRate, 0.05f, 0.3f); // Keep it within bounds
        lastBestFitness = bestFitness;

        for (int i = 0; i < networks.Count; i++)
        {
            if (i == 0)
                networks[i] = sortedCars[i].GetNeuralNetwork();
            else
                networks[i] = sortedCars[0].GetNeuralNetwork().Mutate(mutationRate);
        }
    }

}
