using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public float pitch;
    public float volume;
    public bool Loop;
    [HideInInspector]
    public AudioSource audioSource;
}
