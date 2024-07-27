using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string gameplaySceneName = "Day1"; // Nama scene gameplay

    // Metode ini akan dipanggil saat tombol diklik
    public void SwitchToGameplay()
    {

        SceneManager.LoadScene(gameplaySceneName);
    }
}
