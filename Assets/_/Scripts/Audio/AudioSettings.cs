using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    // The 'key' that PlayerPrefs should save the player's music volume settings at
    private static string musicVolumeKey = "musicVolume";

    // The 'key' that PlayerPrefs should save the player's music volume settings at
    private static string musicMutedKey = "musicIsMuted";

    // The 'key' that PlayerPrefs should save the player's SFX volume settings at
    private static string sfxVolumeKey = "sfxVolume";

    // The 'key' that PlayerPrefs should save the player's SFX volume settings at
    private static string sfxMutedKey = "sfxIsMuted";

    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle musicMuteToggle;
    [SerializeField] private Toggle sfxMuteToggle;
    
    

    /*
     * INITIALIZATION
     */
    
    private void Awake()
    {
        TryRunDebugWarnings();
        TryInitializeSliderValues();
        TryInitializeToggleValues();
    }

    private void TryRunDebugWarnings()
    {
        if (musicSlider == null)
        {
            Debug.LogWarning("AudioSettings musicSlider must be initialized in the inspector!");
        }

        if (sfxSlider == null)
        {
            Debug.LogWarning("AudioSettings sfxSlider must be initialized in the inspector!");
        }

        if (musicMuteToggle == null)
        {
            Debug.LogWarning("AudioSettings musicMuteToggle must be initialized in the inspector!");
        }

        if (sfxMuteToggle == null)
        {
            Debug.LogWarning("AudioSettings sfxMuteToggle must be initialized in the inspector!");
        }
    }

    private void TryInitializeSliderValues()
    {
        float musicVolume = AudioSettings.GetSavedMusicVolume();
        float sfxVolume = AudioSettings.GetSavedSfxVolume();

        if (musicVolume != null)
        {
            musicSlider.value = musicVolume;
            UpdateMusicVolume(musicVolume);
        }

        if (sfxVolume != null)
        {
            sfxSlider.value = sfxVolume;
            UpdateSfxVolume(sfxVolume);
        }
    }

    private void TryInitializeToggleValues()
    {
        if (musicMuteToggle != null)
        {
            musicMuteToggle.isOn = AudioSettings.SavedMusicIsMuted();
        }

        if (sfxMuteToggle != null)
        {
            sfxMuteToggle.isOn = AudioSettings.SavedSfxIsMuted();
        }
    }



    /*
     * UPDATING SETTINGS
     */
    
    // Music
    public void UpdateMusicVolume(float value)
    {
        UpdateMusicVolume(value, true);
    }

    public void UpdateMusicVolume(float value, bool shouldSave)
    {
        AudioController.UpdateMusicVolume(value);
        musicMuteToggle.isOn = false;

        if (shouldSave)
        {
            SetMusicVolume(value);
        }
    }
    
    // SFX
    public void UpdateSfxVolume(float value)
    {
        UpdateSfxVolume(value, true);
    }

    public void UpdateSfxVolume(float value, bool shouldSave)
    {
        AudioController.UpdateSfxVolume(value);
        sfxMuteToggle.isOn = false;

        if (shouldSave)
        {
            SetSfxVolume(value);
        }
    }
    
    // Toggles
    public void ToggleMuteMusic(bool shouldMute)
    {
        if (shouldMute)
        {
            AudioController.MuteMusic();
            SetMusicMuted(true);
        }

        else
        {
            UpdateMusicVolume(musicSlider.value, false);
            SetMusicMuted(false);
        }
    }

    public void ToggleMuteSfx(bool shouldMute)
    {
        if (shouldMute)
        {
            AudioController.MuteSfx();
            SetSfxMuted(true);
        }

        else
        {
            UpdateSfxVolume(sfxSlider.value, false);
            SetSfxMuted(false);
        }
    }



    /*
     * SETTERS
     */
    
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(musicVolumeKey, value);
    }

    public void SetMusicMuted(bool isMuted)
    {
        int musicIsMuted = 0;

        if (isMuted)
        {
            musicIsMuted = 1;
        }

        PlayerPrefs.SetInt(musicMutedKey, musicIsMuted);
    }

    public void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat(sfxVolumeKey, value);
    }

    public void SetSfxMuted(bool isMuted)
    {
        int sfxIsMuted = 0;

        if (isMuted)
        {
            sfxIsMuted = 1;
        }

        PlayerPrefs.SetInt(sfxMutedKey, sfxIsMuted);
    }

    public void SaveSettings()
    {
        PlayerPrefs.Save();
    }
    
    
    
    /*
     * GETTERS
     */
    
    public static float GetSavedMusicVolume()
    {
        return PlayerPrefs.GetFloat(musicVolumeKey, 1f);
    }

    public static float GetSavedSfxVolume()
    {
        return PlayerPrefs.GetFloat(sfxVolumeKey, 1f);
    }
    
    

    /*
     * BOOLEANS
     */
    
    public static bool SavedMusicIsMuted()
    {
        int isMuted = PlayerPrefs.GetInt(musicMutedKey);

        if (isMuted == null)
        {
            return false;
        }

        return (isMuted == 1);
    }

    public static bool SavedSfxIsMuted()
    {
        int isMuted = PlayerPrefs.GetInt(sfxMutedKey);
        
        if (isMuted == null)
        {
            return false;
        }
        
        return (isMuted == 1);
    }
}
