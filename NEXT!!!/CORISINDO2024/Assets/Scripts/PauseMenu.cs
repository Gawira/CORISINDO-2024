using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Reference to the Pause Menu UI
    public static bool isPaused = false;  // Game state (paused or not)
    public ObjectInteractor objectInteractor; // Reference to the ObjectInteractor script

    void Start()
    {
        if (objectInteractor == null)
        {
            Debug.LogError("ObjectInteractor not assigned in the inspector.");
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        if (objectInteractor != null)
        {
            objectInteractor.SetRaycastEnabled(true); // Enable raycasting
        }
        Time.timeScale = 1f;  // Resume the game
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        if (objectInteractor != null)
        {
            objectInteractor.SetRaycastEnabled(false); // Disable raycasting
        }
        Time.timeScale = 0f;  // Pause the game
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;  // Resume time before returning to the main menu
        SceneManager.LoadScene("Main menu FIX");  // Change "MainMenu" to your main menu scene name
    }

    public void LoadSettings()
    {
        // Add code to load the settings menu
    }

    public void LoadHowToPlay()
    {
        // Add code to load the how-to-play menu
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
