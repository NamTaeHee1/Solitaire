using UnityEngine;

public class ScreenResolution : MonoBehaviour
{
    void Start()
    {
        SetCameraResolution();
    }

    private void SetCameraResolution()
    {
        Camera mainCam = Camera.main;
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
