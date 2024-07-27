using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;  // Referensi ke UI Pause Menu
    public static bool isPaused = false;  // Status permainan (pause atau tidak)

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
        Time.timeScale = 1f;  // Lanjutkan permainan
        isPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  // Hentikan permainan
        isPaused = true;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;  // Lanjutkan waktu sebelum kembali ke menu utama
        SceneManager.LoadScene("Main menu FIX");  // Ganti "MainMenu" dengan nama scene menu utama Anda
    }

    public void LoadSettings()
    {
        // Tambahkan kode untuk memuat menu pengaturan
    }

    public void LoadHowToPlay()
    {
        // Tambahkan kode untuk memuat menu cara bermain
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
