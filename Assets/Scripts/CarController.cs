using UnityEngine;
using System.Collections;

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
    private bool isDestroyed = false;
    public int lapCount = 0;


    private Rigidbody rb;
    private NeuralNetwork network;
    private Transform[] waypoints;
    private int nextCheckpointIndex = 0;
    private float fitness = 0f;

    private Vector3 startPosition;   // Stores the initial spawn position
    private Quaternion startRotation; // Stores the initial spawn rotation


    private float checkpointReward = 500f;
    private float checkpointDetectionRadius = 7.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogError($"{gameObject.name} has no waypoints assigned!"); // ✅ Debug missing waypoints
        }

        if (network == null)
        {
            Debug.LogError($"{gameObject.name} has no Neural Network assigned!"); // ✅ Debug missing network
        }
        
        if (waypoints != null && waypoints.Length > 0)
        {
            nextCheckpointIndex = 0; // ✅ Ensure the first checkpoint is targeted

            // ✅ Rotate the car to face the first checkpoint
            Vector3 directionToFirstCheckpoint = (waypoints[0].position - transform.position).normalized;
            directionToFirstCheckpoint.y = 0; // Keep car level
            transform.rotation = Quaternion.LookRotation(directionToFirstCheckpoint);
        }
        else
        {
            Debug.LogError($"{gameObject.name} has no waypoints assigned!");
        }
    }


    

    private void FixedUpdate()
    {
        if (isDestroyed || network == null || waypoints == null || waypoints.Length == 0) return; // ✅ Prevent errors

        float[] inputs = GetSensorInputs();
        float[] outputs = network.ForwardPass(inputs);

        float throttle = outputs[0];
        float steering = outputs[1];

        ApplyAcceleration(throttle);
        ApplySteering(steering);
        ApplyDownforce();
        ApplyDrag();

        DetectCheckpoint();
    }

    private float[] GetSensorInputs()
    {
        if (waypoints.Length == 0) return new float[8]; // Avoid errors if no waypoints exist

        Transform nextCheckpoint = waypoints[nextCheckpointIndex];
        Vector3 directionToCheckpoint = (nextCheckpoint.position - transform.position).normalized;
        float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);

        return new float[]
        {
            currentSpeed / topSpeed,                   // Speed ratio
            fuel / 50f,                                // Fuel level ratio
            tyreGrip,                                  // Tyre grip
            acceleration / 10f,                        // Acceleration ratio
            downforce / 500f,                          // Downforce ratio
            directionToCheckpoint.x,                   // X direction to next checkpoint
            directionToCheckpoint.z,                   // Z direction to next checkpoint
            Mathf.Clamp01(distanceToCheckpoint / 100f) // Normalised distance to checkpoint
        };
    }
    



    private void DetectCheckpoint()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform nextCheckpoint = waypoints[nextCheckpointIndex];
        float distanceToCheckpoint = Vector3.Distance(transform.position, nextCheckpoint.position);

        if (distanceToCheckpoint <= checkpointDetectionRadius)
        {
            fitness += checkpointReward;

            // ✅ Move to the next checkpoint (looping back to 0 if at the last checkpoint)
            nextCheckpointIndex = (nextCheckpointIndex + 1) % waypoints.Length;

            Debug.Log($"✅ {gameObject.name} reached checkpoint {nextCheckpointIndex}! Fitness: {fitness}");
        }
    }


    public void SetWaypoints(Transform[] newWaypoints)
    {
        waypoints = newWaypoints;
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
    public int GetLapsCompleted()
    {
        return lapCount;
    }
    public void ResetCar()
    {
        if (rb == null) rb = GetComponent<Rigidbody>(); // Ensure Rigidbody is assigned
        
        rb.velocity = Vector3.zero; // Stop movement
        rb.angularVelocity = Vector3.zero; // Stop rotation
        lapCount = 0; // Reset lap count
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
        float speedFactor = Mathf.Clamp01(1 - (currentSpeed / topSpeed));
        float maxTurnAngle = Mathf.Lerp(10f, 30f, speedFactor);
        float turnAngle = Mathf.Clamp(steeringInput * maxTurnAngle, -maxTurnAngle, maxTurnAngle);
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

    /// <summary>
    /// Handles car destruction when colliding with walls.
    /// </summary>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            float impactForce = collision.relativeVelocity.magnitude;
            fitness -= impactForce * 50f; // ✅ Apply fitness penalty for crashing

            Debug.Log($"❌ {gameObject.name} crashed into a wall! Impact Force: {impactForce}, Fitness: {fitness}");
            DestroyCar();
        }
    }

    public void DestroyCar()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        gameObject.SetActive(false);

        if (CarEvolutionManager.instance != null)
        {
            CarEvolutionManager.instance.NotifyCarDestroyed(this); // ✅ Notify Evolution Manager
        }
    }

    public float GetFitness()
    {
        return fitness;
    }

    public NeuralNetwork GetNeuralNetwork()
    {
        return network;
    }

    public void SetNeuralNetwork(NeuralNetwork net)
    {
        network = net;
    }
}
