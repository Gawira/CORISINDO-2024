using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioSettings : MonoBehaviour
{
    public Slider overallVolumeSlider;
    public Slider sfxVolumeSlider;
    public Slider musicVolumeSlider;

    public TMP_Text overallVolumeText;
    public TMP_Text sfxVolumeText;
    public TMP_Text musicVolumeText;

    private void Start()
    {
        LoadSettings();

        overallVolumeSlider.onValueChanged.AddListener(delegate { UpdateOverallVolume(); });
        sfxVolumeSlider.onValueChanged.AddListener(delegate { UpdateSFXVolume(); });
        musicVolumeSlider.onValueChanged.AddListener(delegate { UpdateMusicVolume(); });
    }

    private void LoadSettings()
    {
        overallVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        overallVolumeText.text = Mathf.RoundToInt(overallVolumeSlider.value * 100).ToString();
        sfxVolumeText.text = Mathf.RoundToInt(sfxVolumeSlider.value * 100).ToString();
        musicVolumeText.text = Mathf.RoundToInt(musicVolumeSlider.value * 100).ToString();
    }

    private void UpdateOverallVolume()
    {
        float volume = overallVolumeSlider.value;
        overallVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.Master);
        Debug.Log("Overall volume changed to: " + volume);
    }

    private void UpdateSFXVolume()
    {
        float volume = sfxVolumeSlider.value;
        sfxVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.SFX);
        Debug.Log("SFX volume changed to: " + volume);
    }

    private void UpdateMusicVolume()
    {
        float volume = musicVolumeSlider.value;
        musicVolumeText.text = Mathf.RoundToInt(volume * 100).ToString();
        AudioManager.Instance.SetVolume(volume, AudioManager.AudioChannel.Music);
        Debug.Log("Music volume changed to: " + volume);
    }
}
