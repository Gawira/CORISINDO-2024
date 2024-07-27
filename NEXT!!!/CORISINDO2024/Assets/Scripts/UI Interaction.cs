using UnityEngine;

public class UIInteraction : MonoBehaviour
{
    public GameObject popupHTP;  // Referensi ke UI Popup
    public GameObject popupHTP1;
    public GameObject popupHTP2;

    public GameObject popupSetting;  // Referensi ke UI Popup
    public GameObject popupSetting1;
    public GameObject popupSetting2;

    public GameObject audioSettingsPanel;  // Reference to the audio settings panel


    public AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip clickSound;  // Reference to the click sound effect

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Fungsi untuk menampilkan popup
    public void ShowPopup()
    {
        PlayClickSound();
        popupHTP.SetActive(true);
        popupHTP1.SetActive(true);
        popupHTP2.SetActive(true);

    }

    public void ShowPopupSetting()
    {
        PlayClickSound();
        popupSetting.SetActive(true);
        popupSetting1.SetActive(true);
        popupSetting2.SetActive(true);
        audioSettingsPanel.SetActive(true);  // Show audio settings panel
    }

    // Fungsi untuk menyembunyikan popup
    public void HidePopup()
    {
        PlayClickSound();
        popupHTP.SetActive(false);
        popupHTP1.SetActive(false);
        popupHTP2.SetActive(false);
        popupSetting.SetActive(false);
        popupSetting1.SetActive(false);
        popupSetting2.SetActive(false);
        audioSettingsPanel.SetActive(false);  // Hide audio settings panel
    }

    // Fungsi untuk toggle popup
    public void TogglePopup()
    {
        popupHTP.SetActive(!popupHTP.activeSelf);
    }
}
