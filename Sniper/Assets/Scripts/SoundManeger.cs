using System;
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
            sound.name = s.name + " " + "Sound";
            sound.transform.position = Vector3.zero;
            s.audioSource = sound.AddComponent<AudioSource>();
            s.audioSource.clip = s.clip;
            s.audioSource.volume = s.volume;
            s.audioSource.pitch = s.pitch;
            s.audioSource.panStereo = s.stereopan;
        }
    }
    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            return;
        }
        s.audioSource.Play(); 
    }


}
