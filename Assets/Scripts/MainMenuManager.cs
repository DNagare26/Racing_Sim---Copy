using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the main menu, handling scene transitions and UI navigation.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    public GameObject guidePanel; // UI Panel for instructions
    public GameObject settingsPanel; // UI Panel for settings

    private void Start()
    {
        // Ensure guide and settings panels are hidden at start
        if (guidePanel != null) guidePanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Loads the Car Configuration scene.
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("CarConfig");
    }

    /// <summary>
    /// Opens the guide panel.
    /// </summary>
    public void OpenGuide()
    {
        if (guidePanel != null) guidePanel.SetActive(true);
        SceneManager.LoadScene("Guide");
    }

    /// <summary>
    /// Closes the guide panel.
    /// </summary>
    public void CloseGuide()
    {
        if (guidePanel != null) guidePanel.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Opens the settings panel.
    /// </summary>
    public void OpenSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(true);
        SceneManager.LoadScene("Settings");
    }

    /// <summary>
    /// Closes the settings panel.
    /// </summary>
    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Exits the game (Only works in a built version).
    /// </summary>
    public void ExitGame()
    {
        
        Application.Quit();
        Debug.Log("Exiting game...");
    }
}