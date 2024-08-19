using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private GameObject sfxParent;
    [SerializeField] private GameObject musicParent;

    private readonly Dictionary<string, AudioSource> sfxSources = new Dictionary<string, AudioSource>();
    private readonly Dictionary<string, AudioSource> musicSources = new Dictionary<string, AudioSource>();

    public static AudioController Instance => AudioController.instance;
    private static AudioController instance;

    private float globalVolume;
    private float musicVolume;
    private float sfxVolume;

    [Header("Settings")]
    [SerializeField] private bool initializeAudioVolumesOnStart = true;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        this.globalVolume = AudioSettings.GetSavedGlobalVolume();
        this.musicVolume = AudioSettings.GetSavedMusicVolume();
        this.sfxVolume = AudioSettings.GetSavedSfxVolume();

        // Initialize all SFX
        var allSfx = this.sfxParent.GetComponentsInChildren<AudioSource>();
        foreach(var sfx in allSfx)
        {
            if (this.sfxSources.ContainsKey(sfx.name))
            {
                Debug.LogWarning($"Sound Effect with name {sfx.name} already added to effects list.");
                continue;
            }
            this.sfxSources.Add(sfx.name, sfx);
        }

        // Initialize all music
        var allMusic = this.musicParent.GetComponentsInChildren<AudioSource>();
        foreach (var music in allMusic)
        {
            this.musicSources.Add(music.name, music);
        }

        // Initialize volumes
        if (initializeAudioVolumesOnStart && !AudioInitializer.IsInitialized)
        {
            AudioInitializer.TryInitializeAudioVolumes();
        }
    }

    public static void PlaySfx(string sfxName, bool shouldntPlayIfPlaying = false)
    {
        if (instance != null && instance.sfxSources.ContainsKey(sfxName))
        {
            if (shouldntPlayIfPlaying && instance.sfxSources[sfxName].isPlaying)
            {
                return;
            }
            
            instance.sfxSources[sfxName].Play();
        }
    }

    public static void ToggleSfx(string sfxName)
    {
        if (instance != null && instance.sfxSources.ContainsKey(sfxName))
        {
            if (instance.sfxSources[sfxName].isPlaying)
            {
                instance.sfxSources[sfxName].Stop();
            }
            
            else
            {
                instance.sfxSources[sfxName].Play();
            }
        }
    }

    public static void StopSfx(string sfxName)
    {
        if (instance != null && instance.sfxSources.ContainsKey(sfxName))
        {
            instance.sfxSources[sfxName].Stop();
        }
    }

    public static void PlayMusic(string music)
    {
        if (instance != null && instance.musicSources.ContainsKey(music))
        {
            if (instance.musicSources[music].isPlaying)
            {
                return;
            }
            
            instance.musicSources[music].Play();
        }
    }

    public static void StopMusic(string music)
    {
        if (instance != null && instance.musicSources.ContainsKey(music))
        {
            instance.musicSources[music].Stop();
        }
    }

    public static void FadeMusic(string music, float volume)
    {
        if (instance != null && instance.musicSources.ContainsKey(music))
        {
            instance.musicSources[music].volume = volume;
        }
    }

    public static float GetMusicVolume(string music)
    {
        if (instance != null && instance.musicSources.ContainsKey(music))
        {
            return instance.musicSources[music].volume;
        }
        
        return 0;
    }

    public static void UpdateGlobalVolume(float newValue)
    {
        instance.globalVolume = newValue;
        AudioSettings.SetGlobalVolume(instance.globalVolume);

        UpdateMixer("GlobalVolume", instance.globalVolume);
    }

    public static void UpdateMusicVolume(float newValue)
    {
        instance.musicVolume = newValue;
        AudioSettings.SetMusicVolume(instance.musicVolume);

        UpdateMixer("MusicVolume", instance.musicVolume);
    }

    public static void UpdateSfxVolume(float newValue)
    {
        instance.sfxVolume = newValue;
        AudioSettings.SetSfxVolume(instance.musicVolume);

        UpdateMixer("SfxVolume", instance.sfxVolume);
    }

    private static void UpdateMixer(string id, float value)
    {
        var dbVolume = Mathf.Log10(value) * 20;
        
        if (value == 0.0f)
        {
            dbVolume = -80.0f;
        }

        instance.audioMixer.SetFloat(id, dbVolume);
    }

    public static void MuteMusic()
    {
        instance.musicVolume = 0;
        AudioSettings.SetMusicMuted(true);

        UpdateMixer("MusicVolume", instance.musicVolume);
    }

    public static void MuteSfx()
    {
        instance.sfxVolume = 0;
        AudioSettings.SetSfxMuted(true);

        UpdateMixer("SfxVolume", instance.sfxVolume);
    }
}
