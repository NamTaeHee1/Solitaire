using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    [Header("Audio Source")][SerializeField]
    private AudioSource audioSource;

    /// <summary>
    /// Resource 재호출을 방지하기 위한 Audio Clip 저장용 변수
    /// </summary>
    private Dictionary<string, AudioClip> audioDicts = new Dictionary<string, AudioClip>();

    public void Play(string path)
    {
        path = $"Sounds/{path}";

        AudioClip clip;

        if (audioDicts.TryGetValue(path, out clip) == false)
        {
            clip = Resources.Load<AudioClip>(path);
            audioDicts.Add(path, clip);
        }

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
