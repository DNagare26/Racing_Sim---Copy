using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CarEvolutionManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject carPrefab;
    public int populationSize = 10;
    public float maxGenerationTime = 120f;
    public Transform[] spawnPoints;
    public float timeScale = 2.0f;
    public string nextSceneName = "RaceScene"; // Scene for racing

    private Dictionary<CarData, List<CarAgent>> carTypeGroups = new Dictionary<CarData, List<CarAgent>>();
    private float timer;
    private int generationCount;
    private bool trainingStopped = false;

    public static CarEvolutionManager instance;

    void Start()
    {
        if (instance == null) instance = this;

        Time.timeScale = timeScale;
        timer = maxGenerationTime;
        InitializeCars();
        StartCoroutine(TrainingLoop());
    }

    private void InitializeCars()
    {
        var carConfigs = GameManager.instance.GetCarConfigs();
        foreach (var config in carConfigs)
        {
            if (!carTypeGroups.ContainsKey(config))
                carTypeGroups[config] = new List<CarAgent>();

            for (int i = 0; i < populationSize; i++)
            {
                NeuralNetwork network = new NeuralNetwork(5, 8, 4); 

                CarAgent agent = new CarAgent(config, network);
                carTypeGroups[config].Add(agent);
            }
        }
    }

    private IEnumerator TrainingLoop()
    {
        while (generationCount < 50 && !trainingStopped)
        {
            yield return new WaitForSeconds(1);
            EvolveGeneration();
        }
        
    }

    private void EvolveGeneration()
    {
        Dictionary<CarData, List<CarAgent>> newCarTypeGroups = new Dictionary<CarData, List<CarAgent>>(); // ✅ Temporary dictionary

        foreach (var carGroup in carTypeGroups)
        {
            List<CarAgent> agents = carGroup.Value;
            agents.Sort((a, b) => b.GetFitness().CompareTo(a.GetFitness()));

            List<CarAgent> newGeneration = new List<CarAgent> { agents[0] }; // Keep the best one

            for (int i = 1; i < agents.Count; i++)
            {
                NeuralNetwork parent = agents[Random.Range(0, agents.Count / 2)].network;
                float mutationRate = Mathf.Lerp(0.1f, 0.5f, generationCount / 20f);
                NeuralNetwork child = parent.Mutate(mutationRate);
                newGeneration.Add(new CarAgent(carGroup.Key, child));
            }

            newCarTypeGroups[carGroup.Key] = newGeneration; // ✅ Store in the temporary dictionary
        }

        carTypeGroups = newCarTypeGroups; // ✅ Replace the old dictionary with the updated one
        generationCount++;
    }


    public void SaveTrainedModels()
    {
        List<NeuralNetwork> bestNetworks = new List<NeuralNetwork>();

        foreach (var carGroup in carTypeGroups)
        {
            bestNetworks.Add(carGroup.Value[0].network); // Save the best AI for each car type
        }

        GameManager.instance.SetFinalAIModels(bestNetworks);
        Debug.Log("Trained AI Models Saved!");
    }

    public void EndTraining()
    {
        Debug.Log("Training Complete. Moving to Next Scene.");
        SceneManager.LoadScene("RaceScene"); // Ensure "RaceScene" is the correct scene name
    }


    public void StopTraining()
    {
        SaveTrainedModels();
        EndTraining();
        trainingStopped = true;
        Debug.Log("Training Stopped by User.");
    }
    
}
