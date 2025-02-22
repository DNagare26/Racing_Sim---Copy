using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays final race results and statistics.
/// </summary>
public class ResultsManager : MonoBehaviour
{
    public TextMeshProUGUI resultsText;
    public TextMeshProUGUI bestCarText;
    public TextMeshProUGUI fastestLapText;
    public TextMeshProUGUI averageSpeedText;
    public TextMeshProUGUI averageAccelerationText;
    
    private void Start()
    {
        DisplayResults();
    }

    private void DisplayResults()
    {
        Queue<RaceResult> results = GameManager.instance.GetRaceResults();
        if (results == null || results.Count == 0)
        {
            resultsText.text = "No results available.";
            return;
        }

        RaceResult bestCar = null;
        float fastestLap = float.MaxValue;
        float totalSpeed = 0f;
        float totalAcceleration = 0f;
        int carCount = 0;

        string resultSummary = "";

        foreach (RaceResult result in results)
        {
            carCount++;
            totalSpeed += result.car.GetComponent<Rigidbody>().velocity.magnitude * 3.6f;
            totalAcceleration += result.car.acceleration;

            if (result.totalTime < fastestLap)
            {
                fastestLap = result.totalTime;
                bestCar = result;
            }

            resultSummary += $"Car {carCount}: {result.lapsCompleted} Laps, Time: {result.totalTime:F2}s\n";
        }

        resultsText.text = resultSummary;
        bestCarText.text = $"Best Car: Car {results.Count - results.Count + 1}";
        fastestLapText.text = $"Fastest Lap: {fastestLap:F2}s";
        averageSpeedText.text = $"Avg Speed: {(totalSpeed / carCount):F1} km/h";
        averageAccelerationText.text = $"Avg Acceleration: {(totalAcceleration / carCount):F2} m/s²";
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}