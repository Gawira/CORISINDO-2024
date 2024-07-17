using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public string gameplaySceneName = "SampleScene"; // Nama scene gameplay

    // Metode ini akan dipanggil saat tombol diklik
    public void SwitchToGameplay()
    {
        SceneManager.LoadScene(gameplaySceneName);
    }
}
