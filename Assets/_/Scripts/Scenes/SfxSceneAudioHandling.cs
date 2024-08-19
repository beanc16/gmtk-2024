using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SfxSceneAudioHandling : MonoBehaviour
{
    public void PlayButtonClickSfx()
    {
        AudioController.PlaySfx("Click Button");
    }

    public void PlayButtonHoverSfx()
    {
        AudioController.PlaySfx("Hover Over Button");
    }
}
