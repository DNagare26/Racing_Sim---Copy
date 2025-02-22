using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages game-wide settings and stores configurations.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private int numberOfCars = 10; // Default to 10

    private List<CarData> carConfigs = new List<CarData>();
    private Queue<RaceResult> raceResults = new Queue<RaceResult>();
    private List<NeuralNetwork> finalAIModels = new List<NeuralNetwork>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetNumberOfCars(int count)
    {
        numberOfCars = Mathf.Clamp(count, 1, 10);
    }

    public int GetNumberOfCars()
    {
        return numberOfCars;
    }

    public void SetCarConfigs(List<CarData> configs)
    {
        carConfigs = configs;
    }

    public List<CarData> GetCarConfigs()
    {
        return carConfigs;
    }

    public void SetFinalAIModels(List<NeuralNetwork> models)
    {
        finalAIModels = models;
    }

    public List<NeuralNetwork> GetFinalAIModels()
    {
        return finalAIModels;
    }

    public void SetRaceResults(Queue<RaceResult> results)
    {
        raceResults = results;
    }

    public Queue<RaceResult> GetRaceResults()
    {
        return raceResults;
    }
}