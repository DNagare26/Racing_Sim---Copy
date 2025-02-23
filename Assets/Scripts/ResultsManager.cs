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
        int bestCarIndex = -1;

        string resultSummary = "";

        int index = 1;
        foreach (RaceResult result in results)
        {
            carCount++;
            totalSpeed += result.averageSpeed;
            totalAcceleration += result.averageAcceleration;

            if (result.bestLapTime < fastestLap)
            {
                fastestLap = result.bestLapTime;
                bestCar = result;
                bestCarIndex = index;
            }

            resultSummary += $"Car {index}: {result.lapsCompleted} Laps, Time: {result.bestLapTime:F2}s\n";
            index++;
        }

        resultsText.text = resultSummary;

        if (bestCar != null && bestCarIndex != -1)
        {
            bestCarText.text = $"Best Car: Car {bestCarIndex}";
        }
        else
        {
            bestCarText.text = "Best Car: N/A";
        }

        fastestLapText.text = $"Fastest Lap: {fastestLap:F2}s";
        averageSpeedText.text = $"Avg Speed: {(carCount > 0 ? totalSpeed / carCount : 0):F1} km/h";
        averageAccelerationText.text = $"Avg Acceleration: {(carCount > 0 ? totalAcceleration / carCount : 0):F2} m/s²";
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
