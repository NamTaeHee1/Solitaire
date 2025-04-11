using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : SingletonMono<SoundManager>
{
    [Header("Audio Source")][SerializeField]
    private AudioSource audioSource;

    public void Play(string path)
    {
        path = $"Sounds/{path}";

        AudioClip clip;

        clip = ResourcesCache<AudioClip>.Load(path);

        audioSource.PlayOneShot(clip);
    }

    public void SetVolume(ESOUND_STATE state)
    {
        if (state == ESOUND_STATE.ON) 
            audioSource.volume = 1.0f;
        else
            audioSource.volume = 0f;
    }
}
