using System;

/// <summary>
/// Stores race results for each car.
/// </summary>
[Serializable]
public class RaceResult
{
    public CarController car;
    public int lapsCompleted;
    public float totalTime;

    public RaceResult(CarController car, int lapsCompleted, float totalTime)
    {
        this.car = car;
        this.lapsCompleted = lapsCompleted;
        this.totalTime = totalTime;
    }
}