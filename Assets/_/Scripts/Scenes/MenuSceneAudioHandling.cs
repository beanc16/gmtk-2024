using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSceneAudioHandling : MonoBehaviour
{
    private void Start()
    {
        PlayBgm();
    }

    private void PlayBgm()
    {
        AudioController.StopMusic("Game BGM");
        AudioController.PlayMusic("Main Menu BGM");
    }
}
