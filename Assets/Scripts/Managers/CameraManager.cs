using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<CameraManager>();
            }

            return _instance;
        }
    }
    private static CameraManager _instance;

    [HideInInspector]
    public Camera mainCam;

    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void Start()
    {
        SetCameraResolution();
    }

    private void SetCameraResolution()
    {
        Rect viewportRect = mainCam.rect;

        float screenAspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio = 9f / 16f;

        if (screenAspectRatio < targetAspectRatio)
        {
            viewportRect.height = screenAspectRatio / targetAspectRatio;
            viewportRect.y = (1f - viewportRect.height) / 2f;
        }
        else
        {
            viewportRect.width = targetAspectRatio / screenAspectRatio;
            viewportRect.x = (1f - viewportRect.width) / 2f;
        }

        mainCam.rect = viewportRect;
    }
}
