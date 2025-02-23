using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CarConfigManager : MonoBehaviour
{
    public TMP_InputField numberOfCarsInput;
    public GameObject carConfigTemplate;
    public Transform configPanelContainer;
    public Transform tabContainer;
    public GameObject tabButtonTemplate;
    public Button saveButton;

    private List<CarData> carConfigurations = new List<CarData>();
    private List<GameObject> carPanels = new List<GameObject>();
    private List<Button> tabButtons = new List<Button>();

    private void Start()
    {
        saveButton.gameObject.SetActive(false);
    }

    public void GenerateCarConfigurations()
    {
        foreach (Transform child in configPanelContainer) Destroy(child.gameObject);
        foreach (Transform child in tabContainer) Destroy(child.gameObject);
        carConfigurations.Clear();
        carPanels.Clear();
        tabButtons.Clear();

        if (!int.TryParse(numberOfCarsInput.text, out int carCount) || carCount <= 0 || carCount > 10)
        {
            Debug.Log("Invalid number of cars entered! Must be between 1 and 10.");
            carCount = 5;
            return;
        }

        for (int i = 0; i < carCount; i++)
        {
            CarData newCar = new CarData(200, 5, 1000, 50, 1.0f, 0.85f, 2.5f, "Sunny");
            carConfigurations.Add(newCar);

            GameObject panel = Instantiate(carConfigTemplate, configPanelContainer);
            panel.name = $"Car {i + 1} Config";
            panel.SetActive(i == 0);
            carPanels.Add(panel);
            SetupCarPanel(panel, i);

            GameObject tabButtonObj = Instantiate(tabButtonTemplate, tabContainer);
            Button tabButton = tabButtonObj.GetComponent<Button>();
            tabButton.GetComponentInChildren<TextMeshProUGUI>().text = $"Car {i + 1}";
            int index = i;
            tabButton.onClick.AddListener(() => SwitchToCar(index));
            tabButtons.Add(tabButton);
        }

        saveButton.gameObject.SetActive(true);
    }

    private void SetupCarPanel(GameObject panel, int index)
    {
        panel.transform.Find("CarTitle").GetComponent<TextMeshProUGUI>().text = $"Car {index + 1}";

        string[] fields = { "TopSpeedInput", "AccelerationInput", "DownforceInput", "FuelInput", 
                            "TyreGripInput", "AeroEfficiencyInput", "PitStopTimeInput" };
        float[] values = { carConfigurations[index].topSpeed, carConfigurations[index].acceleration, carConfigurations[index].downforce,
                           carConfigurations[index].fuel, carConfigurations[index].tyreGrip, carConfigurations[index].aeroEfficiency, carConfigurations[index].pitStopTime };

        for (int i = 0; i < fields.Length; i++)
        {
            TMP_InputField inputField = panel.transform.Find(fields[i]).GetComponent<TMP_InputField>();
            inputField.text = values[i].ToString();
            int fieldIndex = i;
            inputField.onEndEdit.AddListener(value => UpdateCarConfig(index, fieldIndex, value));
        }

        TMP_Dropdown dropdown = panel.transform.Find("WeatherDropdown").GetComponent<TMP_Dropdown>();
        dropdown.value = dropdown.options.FindIndex(option => option.text == carConfigurations[index].weather);
        dropdown.onValueChanged.AddListener(value => carConfigurations[index].weather = dropdown.options[value].text);
    }

    private void UpdateCarConfig(int carIndex, int fieldIndex, string value)
    {
        if (float.TryParse(value, out float newValue))
        {
            switch (fieldIndex)
            {
                case 0: carConfigurations[carIndex].topSpeed = newValue; break;
                case 1: carConfigurations[carIndex].acceleration = newValue; break;
                case 2: carConfigurations[carIndex].downforce = newValue; break;
                case 3: carConfigurations[carIndex].fuel = newValue; break;
                case 4: carConfigurations[carIndex].tyreGrip = newValue; break;
                case 5: carConfigurations[carIndex].aeroEfficiency = newValue; break;
                case 6: carConfigurations[carIndex].pitStopTime = newValue; break;
            }
        }
    }

    private void SwitchToCar(int index)
    {
        for (int i = 0; i < carPanels.Count; i++)
        {
            carPanels[i].SetActive(i == index);
        }
    }

    public void SaveConfigurations()
    {
        if (carConfigurations.Count == 0)
        {
            Debug.Log("No car configurations to save!");
            return;
        }

        Debug.Log($"Saving {carConfigurations.Count} Car Configurations...");
        GameManager.instance.SetCarConfigs(carConfigurations);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Training");
    }
}
