using UnityEngine;
using UnityEngine.UI;

public class ResetData : MonoBehaviour
{
    public AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip clickSound;  // Reference to the click sound effect
    void Start()
    {
        // Ensure there's a Button component and an OnClick listener
        Button resetButton = GetComponent<Button>();
        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGameData);
        }
        else
        {
            Debug.LogError("No Button component found on this GameObject.");
        }
    }

    public void ResetGameData()
    {
        PlayClickSound();
        // Remove game-related PlayerPrefs keys
        PlayerPrefs.DeleteKey("Days1");
        PlayerPrefs.DeleteKey("TotalMoney");

        // You can add more keys here if you have other game-related values to reset
        PlayerPrefs.DeleteKey("Mistakes");
        PlayerPrefs.DeleteKey("CorrectDecisions");

        // Optionally, reset GameValues instance if it holds in-memory data
        if (GameValues.Instance != null)
        {
            GameValues.Instance.ResetGameValues();
        }

        Debug.Log("Game data reset successfully.");
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

}
