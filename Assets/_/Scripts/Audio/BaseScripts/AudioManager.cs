using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.SoundManagerNamespace;

/// <summary>
/// Manage audio using DigitalRuby's SoundManager
/// </summary>
public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private List<AudioScriptableObject> audioData = new List<AudioScriptableObject>();

    private Dictionary<AudioLayer, Dictionary<AudioName, AudioSource>> audio = new Dictionary<AudioLayer, Dictionary<AudioName, AudioSource>>();

    private void Awake()
    {
        InitializeAudio();
    }

    private void InitializeAudio()
    {
        // Initialize an empty dictionary for each audio layer
        foreach (AudioLayer audioLayer in Enum.GetValues(typeof(AudioLayer)))
        {
            audio.Add(audioLayer, new Dictionary<AudioName, AudioSource>());
        }

        // Add each audio source to the dictionary
        foreach(AudioScriptableObject audioScriptableObject in audioData)
        {
            audio[audioScriptableObject.Layer].Add(
                audioScriptableObject.Name,
                audioScriptableObject.Source
            );
        }
    }

    /// <summary>
    /// Play an audio clip once using the global sound volume as a multiplier
    /// </summary>
    /// <param name="layer">The layer of the audio</param>
    /// <param name="name">The name of the audio</param>
    public void PlayOnce(AudioLayer layer, AudioName name)
    {
        PlayOnce(layer, name, 1.0f);
    }

    /// <summary>
    /// Play an audio clip once using the global sound volume as a multiplier
    /// </summary>
    /// <param name="layer">The layer of the audio</param>
    /// <param name="name">The name of the audio</param>
    /// <param name="volumeScale">Additional volume scale</param>
    public void PlayOnce(AudioLayer layer, AudioName name, float volumeScale)
    {
        audio[layer][name].PlayOneShotSoundManaged(audio[layer][name].clip, volumeScale);
    }

    /// <summary>
    /// Play a music track and loop it until stopped, using the global music volume as a modifier
    /// </summary>
    /// <param name="layer">The layer of the audio</param>
    /// <param name="name">The name of the audio</param>
    public void PlayLoop(AudioLayer layer, AudioName name)
    {
        PlayLoop(layer, name, 1.0f, 1.0f, true);
    }

    /// <summary>
    /// Play a music track and loop it until stopped, using the global music volume as a modifier
    /// </summary>
    /// <param name="layer">The layer of the audio</param>
    /// <param name="name">The name of the audio</param>
    /// <param name="volumeScale">Additional volume scale</param>
    /// <param name="fadeSeconds">The number of seconds to fade in and out</param>
    /// <param name="persistBetweenScenes">Whether to persist the looping audio between scene changes</param>
    public void PlayLoop(
        AudioLayer layer,
        AudioName name,
        float volumeScale,
        float fadeSeconds,
        bool persistBetweenScenes
    )
    {
        audio[layer][name].PlayLoopingMusicManaged(
            volumeScale,
            fadeSeconds,
            persistBetweenScenes
        );
    }
}
