using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraFollow : MonoBehaviour
{
    private List<Transform> cars = new List<Transform>(); // ✅ Stores active cars
    public Vector3 offset = new Vector3(0, 5, -10); // ✅ Adjust for a 3rd-person view
    public float smoothSpeed = 5f; // ✅ Smooth transition speed
    private int currentCarIndex = 0; // ✅ Tracks which car is being followed
    private bool searchingForCar = false; // ✅ Prevents constant searching

    private void Start()
    {
        StartCoroutine(WaitForCars()); // ✅ Wait for cars before starting
    }

    private void LateUpdate()
    {
        if (cars.Count == 0 || cars[currentCarIndex] == null)
        {
            if (!searchingForCar) StartCoroutine(WaitBeforeFindingCar()); // ✅ Prevent spam searching
            return;
        }

        // ✅ Smoothly move camera to follow the car
        Vector3 targetPosition = cars[currentCarIndex].position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.LookAt(cars[currentCarIndex]); // ✅ Make camera look at car
        HandleCarSwitching();
    }

    private IEnumerator WaitForCars()
    {
        Debug.Log("Waiting for cars to spawn...");
        while (GameObject.FindGameObjectsWithTag("Car").Length == 0)
        {
            yield return new WaitForSeconds(1f); // ✅ Wait until cars exist
        }
        RefreshCarList();
        Debug.Log("Cars found! Following first available car.");
    }

    public void RefreshCarList()
    {
        cars.Clear();
        GameObject[] carObjects = GameObject.FindGameObjectsWithTag("Car");

        foreach (GameObject car in carObjects)
        {
            cars.Add(car.transform);
        }

        if (cars.Count > 0)
        {
            currentCarIndex = 0; // ✅ Always start on Car 1
            Debug.Log($"Following Car {currentCarIndex + 1}");
        }
        else
        {
            Debug.Log("No cars found for the camera to follow!");
        }
    }

    private IEnumerator WaitBeforeFindingCar()
    {
        searchingForCar = true;
        yield return new WaitForSeconds(2f);
        RefreshCarList();

        if (cars.Count > 0)
        {
            currentCarIndex = Mathf.Clamp(currentCarIndex, 0, cars.Count - 1);
            Debug.Log($"Switched to next available car: {cars[currentCarIndex].name}");
        }
        else
        {
            Debug.Log("No cars available! Waiting...");
        }

        searchingForCar = false;
    }

    private void HandleCarSwitching()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchCar(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchCar(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchCar(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SwitchCar(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) SwitchCar(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) SwitchCar(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) SwitchCar(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) SwitchCar(7);
        if (Input.GetKeyDown(KeyCode.Alpha9)) SwitchCar(8);
        if (Input.GetKeyDown(KeyCode.Alpha0)) SwitchCar(9);
    }

    private void SwitchCar(int carIndex)
    {
        RefreshCarList();

        if (carIndex < cars.Count && cars[carIndex] != null)
        {
            currentCarIndex = carIndex;
            Debug.Log($"Switched to Car {carIndex + 1}");
        }
        else
        {
            Debug.LogWarning($"Car {carIndex + 1} is not available!");
        }
    }
}
