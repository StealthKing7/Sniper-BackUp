using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManeger : MonoBehaviour
{
    public Sound[] sounds;

    void Awake()
    {
        foreach (Sound s in sounds)
        {
            GameObject sound = new GameObject();
            sound.transform.SetParent(gameObject.transform);
            s.audioSource = sound.AddComponent<AudioSource>();
            sound.name = s.clip.ToString() + "Sound";
            sound.transform.position = Vector3.zero;
            //s.audioSource.clip = s.clip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;

        }
    }


}
