using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESoundType
{
    BGM,
    EFFECT,
    COUNT
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<SoundManager>();

            return _instance;
        }
    }
    private static SoundManager _instance;

    [Header("Audio Sources")][SerializeField]
    private AudioSource[] audioSources = new AudioSource[(int)ESoundType.COUNT];

    public void Play(ESoundType soundType, string path)
    {
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/{path}");
        AudioSource audioSource = audioSources[(int)soundType];

        if(clip == null)
        {
            Debug.LogError("Missing Clip");

            return;
        }

        if(soundType == ESoundType.BGM)
        {
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            audioSource.PlayOneShot(clip);
        }
    }

    public void Pause()
    {

    }
}
