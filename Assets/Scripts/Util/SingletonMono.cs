using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            if(_instance == null)
            {
                lock(_lock)
                {
                    _instance = FindObjectOfType<T>();
                }
            }

            return _instance;
        }
    }
    private static T _instance;
}
