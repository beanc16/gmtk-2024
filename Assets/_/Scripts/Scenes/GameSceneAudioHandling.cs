using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneAudioHandling : MonoBehaviour
{
    private void Start()
    {
        PlayBgm();
    }

    private void PlayBgm()
    {
        AudioController.StopMusic("Main Menu BGM");
        AudioController.PlayMusic("Game BGM");
    }
}
