using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public List<AudioSource> sfxSources;
    public List<AudioClip> sfxClips;
    public List<AudioSource> musicSources;
    public List<AudioClip> musicClips;
    public VideoPlayer videoPlayer; // Reference to the VideoPlayer

    private float masterVolume = 1f;
    private float sfxVolume = 1f;
    private float musicVolume = 1f;

    public enum AudioChannel { Master, SFX, Music }

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
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void LoadVolumes()
    {
        masterVolume = PlayerPrefs.GetFloat("OverallVolume", 1f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);

        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        SetVolume(masterVolume, AudioChannel.Master);
        SetVolume(sfxVolume, AudioChannel.SFX);
        SetVolume(musicVolume, AudioChannel.Music);
    }

    public void SetVolume(float volume, AudioChannel channel)
    {
        switch (channel)
        {
            case AudioChannel.Master:
                masterVolume = volume;
                ApplyMasterVolume();
                break;
            case AudioChannel.SFX:
                sfxVolume = volume;
                ApplySFXVolume();
                break;
            case AudioChannel.Music:
                musicVolume = volume;
                ApplyMusicVolume();
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

    private void ApplySFXVolume()
    {
        foreach (var source in sfxSources)
        {
            if (source != null) source.volume = sfxVolume;
        }
    }

    private void ApplyMusicVolume()
    {
        foreach (var source in musicSources)
        {
            if (source != null) source.volume = musicVolume;
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
                sfxSource.volume = sfxVolume;
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

    public void PlayMusic(string musicName, bool loop = true)
    {
        AudioClip clip = musicClips.Find(music => music.name == musicName);
        if (clip != null)
        {
            AudioSource musicSource = musicSources.Find(source => !source.isPlaying);
            if (musicSource != null)
            {
                musicSource.clip = clip;
                musicSource.volume = musicVolume;
                musicSource.loop = loop;
                musicSource.Play();
            }
            else
            {
                Debug.LogWarning("No available music source to play: " + musicName);
            }
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + musicName);
        }
    }
}
