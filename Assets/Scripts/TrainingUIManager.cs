using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages UI elements during AI training, including speed control.
/// </summary>
public class TrainingUIManager : MonoBehaviour
{
    public TextMeshProUGUI generationText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI speedText;
    public Slider speedSlider;

    private float trainingTime = 120f;
    
    private void Start()
    {
        if (speedSlider != null)
        {
            speedSlider.onValueChanged.AddListener(UpdateTimeScale);
        }

        InvokeRepeating(nameof(UpdateUI), 1f, 1f);
    }

    private void UpdateUI()
    {
        generationText.text = "Generation: " + GameManager.instance.GetFinalAIModels().Count;
        timerText.text = "Time: " + Mathf.Ceil(trainingTime).ToString("F0") + "s";
        speedText.text = "Speed: " + Time.timeScale.ToString("F0") + "x";

        trainingTime -= Time.deltaTime;
        if (trainingTime <= 0) CancelInvoke(nameof(UpdateUI));
    }

    private void UpdateTimeScale(float newSpeed)
    {
        Time.timeScale = newSpeed;
        speedText.text = "Speed: " + newSpeed.ToString("F0") + "x";
    }

    public void StopTraining()
    {
        Time.timeScale = 1f; // Reset time scale before switching scenes
        UnityEngine.SceneManagement.SceneManager.LoadScene("RaceScene");
    }
}