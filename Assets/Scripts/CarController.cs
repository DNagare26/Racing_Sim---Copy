using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CarController : MonoBehaviour
{
    [Header("Car Settings")]
    public float topSpeed = 200f;
    public float acceleration = 10f;
    public float downforce = 500f;
    public float fuel = 50f;
    public float tyreGrip = 1.0f;
    public float aeroEfficiency = 0.85f;
    public float pitStopTime = 2.5f;

    [Header("Car Status")]
    public float currentSpeed = 0f;
    private float fuelConsumptionRate = 0.1f;
    private bool isRefuelling = false;
    private bool isDestroyed = false;
    public int lapCount = 0;

    private Rigidbody rb;
    private NeuralNetwork network;
    private Vector3 lastPosition;
    private float distanceTravelled = 0f;
    private Vector3 startPosition;
    private Quaternion startRotation;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true; 
        startPosition = transform.position;  // Store initial position
        startRotation = transform.rotation;  // Store initial rotation
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        lastPosition = transform.position;
    }

    private void FixedUpdate()
    {
        if (isDestroyed || isRefuelling) return;

        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;

        if (network == null)
        {
            Debug.LogError($"Neural Network is NOT assigned to {gameObject.name}!");
            return;
        }

        float[] inputs = GetSensorInputs();
        float[] outputs = network.ForwardPass(inputs);

        float throttle = outputs[0];
        float steering = outputs[1];

        ApplyAcceleration(throttle);
        ApplySteering(steering);
        ApplyDownforce();
        ApplyDrag();
        ApplyFuelConsumption();

        transform.Rotate(0, steering * 3f, 0);
    }

    private void ApplyAcceleration(float throttle)
    {
        float force = acceleration * throttle * rb.mass;
        rb.AddForce(transform.forward * force, ForceMode.Acceleration);

        currentSpeed = rb.velocity.magnitude * 3.6f;
        if (currentSpeed > topSpeed)
        {
            rb.velocity = rb.velocity.normalized * (topSpeed / 3.6f);
        }
    }

    private void ApplySteering(float steeringInput)
    {
        float speedFactor = 1 - (currentSpeed / topSpeed);
        float turnAngle = Mathf.Clamp(steeringInput * speedFactor, -1f, 1f) * 30f;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAngle * Time.deltaTime, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

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
        fuel -= fuelConsumptionRate * Time.deltaTime;
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
        fuel = 50f;
        isRefuelling = false;
    }

    private float[] GetSensorInputs()
    {
        return new float[]
        {
            currentSpeed / topSpeed,
            fuel / 50f,
            tyreGrip,
            acceleration / 10f,
            downforce / 500f
        };
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
        return distanceTravelled * Mathf.Max(0, Vector3.Dot(transform.forward, rb.velocity.normalized));
    }
    
    public void ResetCar()
    {
        if (rb == null) rb = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned

        transform.position = startPosition; // Reset position
        transform.rotation = startRotation; // Reset rotation
        rb.velocity = Vector3.zero; // Stop movement
        rb.angularVelocity = Vector3.zero; // Stop rotation
        lapCount = 0; // Reset lap count
    }

}
