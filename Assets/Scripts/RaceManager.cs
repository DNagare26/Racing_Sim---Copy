using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the race, tracks laps, and updates leaderboard.
/// </summary>
public class RaceManager : MonoBehaviour
{
    public List<CarController> raceCars;
    public int totalLaps = 15;
    public Transform[] checkpoints;

    private Dictionary<CarController, int> lapCounts = new Dictionary<CarController, int>();
    private Dictionary<CarController, float> raceTimes = new Dictionary<CarController, float>();

    private void Start()
    {
        foreach (var car in raceCars)
        {
            lapCounts[car] = 0;
            raceTimes[car] = 0f;
        }

        InvokeRepeating(nameof(UpdateLeaderboard), 1f, 1f);
    }

    private void Update()
    {
        foreach (var car in raceCars)
        {
            raceTimes[car] += Time.deltaTime;

            if (lapCounts[car] >= totalLaps)
            {
                EndRace();
                break;
            }
        }
    }

    private void UpdateLeaderboard()
    {
        raceCars.Sort((a, b) => lapCounts[b].CompareTo(lapCounts[a]));
    }

    private void EndRace()
    {
        Queue<RaceResult> results = new Queue<RaceResult>();

        foreach (var car in raceCars)
        {
            results.Enqueue(new RaceResult(car, lapCounts[car], raceTimes[car]));
        }

        GameManager.instance.SetRaceResults(results);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Results");
    }

    public void RegisterCheckpoint(CarController car, int checkpointIndex)
    {
        if (checkpointIndex == 0 && lapCounts[car] < totalLaps)
        {
            lapCounts[car]++;
        }
    }
}