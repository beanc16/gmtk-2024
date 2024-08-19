using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    private const string GLOBAL_PREFS_KEY = "globalVolume";
    private const string MUSIC_PREFS_KEY = "MusicVolume";
    private const string SFX_PREFS_KEY = "SfxVolume";

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

    private void Start()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        instance = this;

        this.globalVolume = PlayerPrefs.GetFloat(GLOBAL_PREFS_KEY);
        this.musicVolume = PlayerPrefs.GetFloat(MUSIC_PREFS_KEY);
        this.sfxVolume = PlayerPrefs.GetFloat(SFX_PREFS_KEY);

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

    public static void PlaySfx(string sfxName)
    {
        if (instance != null && instance.sfxSources.ContainsKey(sfxName))
        {
            instance.sfxSources[sfxName].Play();
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

        PlayerPrefs.SetFloat(GLOBAL_PREFS_KEY, instance.globalVolume);

        UpdateMixer("GlobalVolume", instance.globalVolume);
    }

    public static void UpdateMusicVolume(float newValue)
    {
        instance.musicVolume = newValue;
        PlayerPrefs.SetFloat(MUSIC_PREFS_KEY, instance.musicVolume);

        UpdateMixer("MusicVolume", instance.musicVolume);
    }

    public static void UpdateSfxVolume(float newValue)
    {
        instance.sfxVolume = newValue;
        PlayerPrefs.SetFloat(SFX_PREFS_KEY, instance.sfxVolume);

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
        UpdateMusicVolume(0);
    }

    public static void MuteSfx()
    {
        UpdateSfxVolume(0);
    }
}
