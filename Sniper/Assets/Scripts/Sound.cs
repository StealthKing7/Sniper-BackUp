using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float pitch;
    public float volume;
    [HideInInspector]
    public AudioSource audioSource;
}
