using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioRandomizer : MonoBehaviour
{
    // This script works on the assumption that your object names in the audio controller are listed as "footsteps1", "footsteps2", "footsteps3", etc. based on the baseAudioName
    [SerializeField] private string baseAudioName;
    [SerializeField] private int numOfRandomSounds = 4;
    [SerializeField] private AudioType audioType = AudioType.Sfx;



    public void PlayRandomSound()
    {
        int index = Random.Range(1, numOfRandomSounds);
        string fileName = baseAudioName + index;

        if (audioType == AudioType.Sfx)
        {
            AudioController.PlaySfx(fileName);
        }

        else if (audioType == AudioType.Music)
        {
            AudioController.PlayMusic(fileName);
        }
    }
}
