using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float topSpeed = 200f; // km/h
    public float acceleration = 10f; // m/s²
    public float downforce = 500f; // Newtons
    public float fuel = 50f; // litres
    public float tyreGrip = 1.0f; // Affects traction
    public float aeroEfficiency = 0.85f; // Affects drag
    public float pitStopTime = 2.5f; // Time to refuel

    [Header("Car Status")]
    public float currentSpeed = 0f;
    private float fuelConsumptionRate = 0.1f; // litres per second
    private bool isRefuelling = false;
    private bool isDestroyed = false;

    private Rigidbody rb;
    private NeuralNetwork network;
    private Vector3 lastPosition;
    private float distanceTravelled = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false; // We handle physics manually
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isDestroyed || isRefuelling) return;

        // Update fitness tracking
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        // Get AI inputs (assuming sensors are used in a separate script)
        float[] inputs = GetSensorInputs();
        float[] outputs = network.ForwardPass(inputs);

        float throttle = outputs[0]; // AI throttle input
        float steering = outputs[1]; // AI steering input

        // Apply car physics
        ApplyAcceleration(throttle);
        ApplySteering(steering);
        ApplyDownforce();
        ApplyDrag();
        ApplyFuelConsumption();

        // Update car rotation
        transform.Rotate(0, steering * 3f, 0);
    }


    private void ApplyAcceleration(float throttle)
    {
        float force = acceleration * Mathf.Clamp(throttle, 0.2f, 1f) * rb.mass;
        rb.AddForce(transform.forward * force, ForceMode.Acceleration);

        if (rb.velocity.magnitude < 0.5f)
        {
            rb.velocity += transform.forward * 1f; // Small boost to prevent spinning
        }

        if (rb.velocity.magnitude * 3.6f > topSpeed)
        {
            rb.velocity = rb.velocity.normalized * (topSpeed / 3.6f);
        }
    }



    private void ApplySteering(float steeringInput)
    {
        float maxSteerAngle = 25f; // Reduce maximum turning
        float turnAngle = steeringInput * maxSteerAngle;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAngle * Time.deltaTime, 0f);
    
        rb.MoveRotation(rb.rotation * turnRotation);
    }


    /// <summary>
    /// Applies aerodynamic downforce to improve grip.
    /// </summary>
    private void ApplyDownforce()
    {
        rb.AddForce(-Vector3.up * downforce, ForceMode.Force);
    }


    private void ApplyDrag()
    {
        float dragForce = aeroEfficiency * rb.velocity.magnitude * rb.velocity.magnitude;
        rb.AddForce(-rb.velocity.normalized * dragForce, ForceMode.Force);
    }


    private void ApplyFuelConsumption()
    {
        fuel -= fuelConsumptionRate * Time.deltaTime * (1f + Mathf.Abs(acceleration / topSpeed));
        if (fuel <= 0)
        {
            StartCoroutine(HandlePitStop());
        }
    }


    private IEnumerator HandlePitStop()
    {
        isRefuelling = true;
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(pitStopTime);
        fuel = 50f; // Reset fuel
        isRefuelling = false;
    }


    private float[] GetSensorInputs()
    {
        return new float[]
        {
            currentSpeed / topSpeed, // Normalised speed
            fuel / 50f, // Normalised fuel
            tyreGrip,
            acceleration / 10f,
            downforce / 500f
        };
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Car"))
        {
            Debug.Log($"💥 Car {gameObject.name} crashed! Destroying...");
            DestroyCar();
        }
    }


    public void DestroyCar()
    {
        isDestroyed = true;
        gameObject.SetActive(false);
    }


    public void SetCarConfig(CarData config)
    {
        topSpeed = config.topSpeed;
        acceleration = config.acceleration;
        downforce = config.downforce;
        fuel = config.fuel;
        tyreGrip = config.tyreGrip;
        aeroEfficiency = config.aeroEfficiency;
        pitStopTime = config.pitStopTime;
    }


    public void SetNeuralNetwork(NeuralNetwork net)
    {
        network = net;
    }


    public float GetFitness()
    {
        return distanceTravelled;
    }
    public NeuralNetwork GetNeuralNetwork()
    {
        return network;
    }

}


