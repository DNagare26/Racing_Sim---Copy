using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CarEvolutionManager : MonoBehaviour
{
    public GameObject carPrefab;
    public Transform[] spawnPoints;
    public Transform[] waypoints;
    public int populationSize;
    public TextMeshProUGUI generationText;

    [Header("Time Scale Controller")]
    public Slider timeScaleSlider; // ✅ Add a slider to control training speed
    
    private int generationCount = 1;
    private int activeCars;
    private List<CarController> previousGenerationCars = new List<CarController>(); 
    private Vector3 startPosition;   // Stores the initial spawn position
    private Quaternion startRotation; // Stores the initial spawn rotation

    
    public static CarEvolutionManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        var carConfigs = GameManager.instance.GetCarConfigs();
    
        if (carConfigs == null || carConfigs.Count == 0)
        {
            Debug.LogError("No car configurations found in GameManager! Cannot start training.");
            return;
        }

        // ✅ Ensure `populationSize` matches the number of car configurations
        populationSize = carConfigs.Count;

        Debug.Log($"✅ Setting populationSize to {populationSize} (matches carConfigs.Count)");

        if (timeScaleSlider != null)
        {
            timeScaleSlider.onValueChanged.AddListener(UpdateTimeScale);
            UpdateTimeScale(timeScaleSlider.value); // Set initial time scale
        }
        
        InitialiseCars();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // ✅ Detect Spacebar press
        {
            Debug.Log("🔄 Spacebar pressed! Ending generation and starting next one...");
            StartNextGeneration();
        }
    }
    public void UpdateTimeScale(float value)
    {
        Time.timeScale = value; // ✅ Adjusts game speed
        Time.fixedDeltaTime = 0.02f / value; // ✅ Adjusts physics updates
        Debug.Log($"🕒 Time Scale Set to: {value}, Fixed Delta Time: {Time.fixedDeltaTime}");
    }

    private void InitialiseCars()
    {
        Debug.Log($"🔄 Starting Generation {generationCount} with {populationSize} cars...");

        var carConfigs = GameManager.instance.GetCarConfigs();
        if (carConfigs == null || carConfigs.Count == 0)
        {
            Debug.LogError("No car configurations found! Cannot start training.");
            return;
        }

        foreach (var car in previousGenerationCars)
        {
            if (car != null) Destroy(car.gameObject);
        }
        previousGenerationCars.Clear();

        activeCars = 0;

        for (int i = 0; i < populationSize; i++)
        {
            CarData config = carConfigs[i];
            GameObject carObj = Instantiate(carPrefab, spawnPoints[i % spawnPoints.Length].position, spawnPoints[i % spawnPoints.Length].rotation);
            CarController cc = carObj.GetComponent<CarController>();

            cc.SetCarConfig(config);
            cc.SetNeuralNetwork(new NeuralNetwork(8, 8, 2)); 
            cc.SetWaypoints(waypoints);
            
            if (waypoints == null || waypoints.Length == 0)
            {
                Debug.LogError("⚠ Waypoints are not assigned to CarEvolutionManager!");
            }
            else
            {
                cc.SetWaypoints(waypoints); // ✅ Ensure waypoints are assigned
            }

            if (cc.GetNeuralNetwork() == null)
            {
                cc.SetNeuralNetwork(new NeuralNetwork(8, 8, 2)); // ✅ Assign a new network if missing
            }

            if (waypoints != null && waypoints.Length > 0)
            {
                // ✅ Rotate car to face the first waypoint at spawn
                Vector3 directionToFirstCheckpoint = (waypoints[0].position - carObj.transform.position).normalized;
                directionToFirstCheckpoint.y = 0; // Keep car level
                Quaternion lookRotation = Quaternion.LookRotation(directionToFirstCheckpoint);
                carObj.transform.rotation = lookRotation;
            }

            previousGenerationCars.Add(cc);
            activeCars++;
        }

        Debug.Log($"🚗 Spawned {activeCars} cars for Generation {generationCount}.");
    }





    public void NotifyCarDestroyed(CarController car)
    {
        activeCars--;
        previousGenerationCars.Remove(car);

        if (activeCars <= 0)
        {
            Debug.Log($"✅ All cars destroyed. Starting next generation...");
            StartCoroutine(WaitBeforeNextGeneration());
        }
    }

    private IEnumerator WaitBeforeNextGeneration()
    {
        yield return new WaitForSeconds(2f);
        StartNextGeneration();
    }

    private void StartNextGeneration()
    {
        generationCount++;
        InitialiseCars();
    }
}
