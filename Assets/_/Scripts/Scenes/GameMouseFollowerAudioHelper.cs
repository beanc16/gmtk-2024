using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMouseFollowerAudioHelper : SfxSceneAudioHandling
{
    private GameSceneAudioHandling gameSceneAudioHandling;

    private void Awake()
    {
        gameSceneAudioHandling = FindObjectOfType<GameSceneAudioHandling>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Mergable>())
        {
            gameSceneAudioHandling.PlayWindBlowingSfx();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Mergable>())
        {
            gameSceneAudioHandling.StopWindBlowingSfx();
        }
    }
}
