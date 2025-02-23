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
        if (carConfigs == null || aiModels == null)
        {
            Debug.LogError("No car configurations or AI models found!");
            return;
        }

        numCars = Mathf.Min(startPositions.Length, carConfigs.Count, aiModels.Count);
        raceCars = new CarController[numCars];

        for (int i = 0; i < numCars; i++)
        {
            GameObject carObj = Instantiate(carPrefab, startPositions[i].position, startPositions[i].rotation);
            CarController cc = carObj.GetComponent<CarController>();

            try
            {
                cc.SetCarConfig(carConfigs[i]);
                cc.SetNeuralNetwork(aiModels[i]);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error initializing car " + i + ": " + ex.Message);
            }

            cc.ResetCar();
            raceCars[i] = cc;
        }

        StartCoroutine(RaceLoop());
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

    private bool CheckRaceCompletion()
    {
        foreach (CarController cc in raceCars)
        {
            if (cc.lapCount < totalLaps) return false;
        }
        return true;
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
        System.Array.Sort(raceCars, (a, b) => b.lapCount.CompareTo(a.lapCount));
        string text = "Leaderboard\n";
        for (int i = 0; i < raceCars.Length; i++)
        {
            text += $"{i + 1}. Car {i + 1} - Laps: {raceCars[i].lapCount}\n";
        }
        leaderboardText.text = text;
    }
}
