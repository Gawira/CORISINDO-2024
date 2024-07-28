using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public List<AudioSource> sfxSources;
    public List<AudioClip> sfxClips;
    public List<AudioSource> musicSources;
    public List<AudioClip> musicClips;
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer

    private float masterVolume = 1f;

    public enum AudioChannel { Master }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (var source in sfxSources)
            {
                DontDestroyOnLoad(source.gameObject);
            }
            foreach (var source in musicSources)
            {
                DontDestroyOnLoad(source.gameObject);
            }
            LoadVolumes();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("OverallVolume", 1f);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        SetVolume(masterVolume, AudioChannel.Master);
    }

    public void SetVolume(float volume, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolume = volume;
                ApplyMasterVolume();
                break;
        }

        PlayerPrefs.SetFloat(channel.ToString() + "Volume", volume);
        PlayerPrefs.Save();
    }

    private void ApplyMasterVolume()
    {
        AudioListener.volume = masterVolume;
        if (videoPlayer != null)
        {
            videoPlayer.SetDirectAudioVolume(0, masterVolume);
        }
    }

    public void PlaySFX(string sfxName)
    {
        AudioClip clip = sfxClips.Find(sfx => sfx.name == sfxName);
        if (clip != null)
        {
            AudioSource sfxSource = sfxSources.Find(source => !source.isPlaying);
            if (sfxSource != null)
            {
                sfxSource.clip = clip;
                sfxSource.volume = masterVolume;
                sfxSource.Play();
            }
            else
            {
                Debug.LogWarning("No available SFX source to play: " + sfxName);
            }
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + sfxName);
        }
    }

    public void PlayMusic(string musicName)
    {
        Debug.Log("PlayMusic called with: " + musicName); // Debug log
        foreach (var source in musicSources)
        {
            if (source.clip != null && source.clip.name == musicName)
            {
                Debug.Log("Enabling and playing: " + source.clip.name); // Debug log
                source.enabled = true;
                if (!source.isPlaying)
                {
                    source.Play();
                }
            }
            else
            {
                Debug.Log("Disabling: " + (source.clip != null ? source.clip.name : "null")); // Debug log
                source.enabled = false;
                source.Stop();
            }
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("OnSceneLoaded called with: " + scene.name); // Debug log
        switch (scene.name)
        {
            case "Main menu FIX":
                PlayMusic("mainmenu_theme");
                break;
            case "Day1 FIX":
                PlayMusic("5 Minute version");
                break;
            case "Day transision":
                PlayMusic("daytransition_theme");
                break;
            default:
                Debug.LogWarning("No music found for scene: " + scene.name);
                break;
        }
    }
}
