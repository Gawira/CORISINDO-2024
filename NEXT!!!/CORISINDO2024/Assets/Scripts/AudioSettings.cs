using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    [SerializeField] private Slider overallVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private TextMeshProUGUI overallVolumeText;
    [SerializeField] private TextMeshProUGUI sfxVolumeText;
    [SerializeField] private TextMeshProUGUI musicVolumeText;

    private void Start()
    {
        overallVolumeSlider.value = PlayerPrefs.GetFloat("OverallVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        UpdateOverallVolume();
        UpdateSFXVolume();
        UpdateMusicVolume();

        overallVolumeSlider.onValueChanged.AddListener(delegate { UpdateOverallVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
    }

    private void UpdateOverallVolume()
    {
        float volume = overallVolumeSlider.value;
        overallVolumeText.text = Mathf.RoundToInt(volume * 100).ToString(); 
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.Master);
        PlayerPrefs.SetFloat("OverallVolume", volume);
    }

    private void UpdateSFXVolume()
    {
        float volume = sfxVolumeSlider.value;
        sfxVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.SFX);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    private void UpdateMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        musicVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.Music);
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }
}
