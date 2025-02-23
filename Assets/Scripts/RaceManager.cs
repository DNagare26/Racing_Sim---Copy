using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform[] startPositions;
    public TextMeshProUGUI leaderboardText;
    public int totalLaps = 15;

    private CarController[] raceCars;
    private int numCars;
    private bool raceFinished = false;

    private void Start()
    {
        var carConfigs = GameManager.instance.GetCarConfigs();
        var aiModels = GameManager.instance.GetFinalAIModels();

        if (carConfigs == null || aiModels == null || carConfigs.Count == 0 || aiModels.Count == 0)
        {
            Debug.LogError("No car configurations or AI models found!");
            return;
        }

        if (carConfigs.Count != aiModels.Count)
        {
            Debug.LogError($"Mismatch! CarConfigs ({carConfigs.Count}) and AI Models ({aiModels.Count}) are not equal.");
            return;
        }

        numCars = carConfigs.Count; // ✅ Ensure one config = one car
        raceCars = new CarController[numCars];

        for (int i = 0; i < numCars; i++)
        {
            GameObject carObj = Instantiate(carPrefab, startPositions[i % startPositions.Length].position, startPositions[i % startPositions.Length].rotation);
            carObj.tag = "Car";

            CarController cc = carObj.GetComponent<CarController>();

            try
            {
                cc.SetCarConfig(carConfigs[i]); // ✅ Assign correct config
                cc.SetNeuralNetwork(aiModels[i]); // ✅ Assign correct AI
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error initializing car {i}: {ex.Message}");
            }

            cc.ResetCar();
            raceCars[i] = cc;
        }

        Debug.Log($"🚗 Race started with {numCars} cars.");
    }




    private IEnumerator RaceLoop()
    {
        while (!raceFinished)
        {
            UpdateLeaderboard();
            raceFinished = CheckRaceCompletion();
            yield return new WaitForSeconds(1f);
        }
        StoreRaceResults();
        SceneManager.LoadScene("Results");
    }

    private void StoreRaceResults()
    {
        Queue<RaceResult> results = new Queue<RaceResult>(); 

        for (int i = 0; i < raceCars.Length; i++)
        {
            RaceResult rr = new RaceResult
            {
                carIndex = i,
                averageSpeed = raceCars[i].GetComponent<Rigidbody>().velocity.magnitude * 3.6f,
                averageAcceleration = raceCars[i].acceleration,
                bestLapTime = 0f // Placeholder, replace with actual lap time tracking
            };
            results.Enqueue(rr);
        }

        GameManager.instance.SetRaceResults(results);
    }

    private void UpdateLeaderboard()
    {
        System.Array.Sort(raceCars, (a, b) => b.GetLapsCompleted().CompareTo(a.GetLapsCompleted()));

        string text = "Leaderboard\n";
        for (int i = 0; i < raceCars.Length; i++)
        {
            text += $"{i + 1}. Car {i + 1} - Laps: {raceCars[i].GetLapsCompleted()}\n";
        }
        leaderboardText.text = text;
    }

    private bool CheckRaceCompletion()
    {
        foreach (CarController cc in raceCars)
        {
            if (cc.GetLapsCompleted() < totalLaps) return false; // ✅ Wait until all cars complete 15 laps
        }
        return true;
    }
}
