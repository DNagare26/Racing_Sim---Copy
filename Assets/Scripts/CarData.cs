using System;

// Stores all configurable attributes for an AI car.
// Used to transfer car settings between scenes.

[Serializable]
public class CarData
{
    public float topSpeed;        // Maximum speed (km/h)
    public float acceleration;    // Acceleration rate (m/s²)
    public float downforce;       // Aerodynamic downforce (N)
    public float fuel;            // Initial fuel amount (litres)
    public float tyreGrip;        // Tyre grip factor (affects handling)
    public float aeroEfficiency;  // Affects drag and top speed
    public float pitStopTime;     // Time spent refuelling (seconds)
    public string weather;        // Weather condition affecting performance

    //Creates a car configuration with the given parameters.
    public CarData(float topSpeed, float acceleration, float downforce, float fuel, float tyreGrip, float aeroEfficiency, float pitStopTime, string weather)
    {
        this.topSpeed = topSpeed;
        this.acceleration = acceleration;
        this.downforce = downforce;
        this.fuel = fuel;
        this.tyreGrip = tyreGrip;
        this.aeroEfficiency = aeroEfficiency;
        this.pitStopTime = pitStopTime;
        this.weather = weather;
    }
}