using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneAudioHandling : SfxSceneAudioHandling
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

    public void ToggleWindBlowingSfx()
    {
        AudioController.ToggleSfx("Wind Blowing");
    }

    public void PlayBubbleMergeSfx()
    {
        AudioController.PlaySfx("Bubbles Merge");
    }
}
