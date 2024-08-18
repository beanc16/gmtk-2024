using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/AudioScriptableObject", order = 1)]
public class AudioScriptableObject : ScriptableObject
{
    [SerializeField]
    private AudioName name;

    [SerializeField]
    private AudioLayer layer;

    [SerializeField]
    private AudioSource source;

    public AudioName Name
    {
        get => name;
    }

    public AudioLayer Layer
    {
        get => layer;
    }

    public AudioSource Source
    {
        get => source;
    }
}
