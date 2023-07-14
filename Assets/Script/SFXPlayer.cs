using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip[] clip;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Add an AudioSource component
    }

    public void Play(string name, float volume = 1.0f)
    {
        for(int i = 0; i < clip.Length; i++)
        {
            if (clip[i].name == name)
            {
                audioSource.PlayOneShot(clip[i], volume);
            }
        }
    }
}
