using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider overallVolumeSlider;
    [SerializeField] private TextMeshProUGUI overallVolumeText;

    public void OnEnable()
    {
        overallVolumeSlider.value = PlayerPrefs.GetFloat("OverallVolume", 1f);

        UpdateOverallVolume();

        overallVolumeSlider.onValueChanged.AddListener(delegate { UpdateOverallVolume(); });
    }

    private void OnDisable()
    {
        overallVolumeSlider.onValueChanged.RemoveAllListeners();
    }

    private void LoadSettings()
    {
        overallVolumeSlider.value = PlayerPrefs.GetFloat("OverallVolume", 1f);

        UpdateOverallVolume();
    }

    private void UpdateOverallVolume()
    {
        float volume = overallVolumeSlider.value;
        overallVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.Master);
        PlayerPrefs.SetFloat("OverallVolume", volume);
        PlayerPrefs.Save();
    }
}
