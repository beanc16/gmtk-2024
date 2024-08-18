using UnityEngine;

public class BubbleGameAudioManager : AudioManager
{
    private void Start()
    {
        PlayBgm();
    }

    public void PlayBgm()
    {
        PlayLoop(AudioLayer.MUSIC, AudioName.BGM);
    }
}
