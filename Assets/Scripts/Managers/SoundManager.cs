using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [Header("Audio Source")][SerializeField]
    private AudioSource audioSource;

    /// <summary>
    /// Resource 재호출을 방지하기 위한 Audio Clip 저장용 변수
    /// </summary>
    private Dictionary<string, AudioClip> audioDicts = new Dictionary<string, AudioClip>();

    public void Play(string path)
    {
        path = $"Sounds/{path}";
        Debug.Log($"path : {path}");

        AudioClip clip;

        if (audioDicts.TryGetValue(path, out clip) == false)
        {
            clip = Resources.Load<AudioClip>(path);
            audioDicts.Add(path, clip);
        }

        audioSource.PlayOneShot(clip);
    }
}
