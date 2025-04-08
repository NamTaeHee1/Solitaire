using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings
{
    public static void Load()
    {
        ApplyTouchState((ETOUCH_STATE)PlayerPrefs.GetInt("Touch", (int)ETOUCH_STATE.ON));
        ApplySoundState((ESOUND_STATE)PlayerPrefs.GetInt("Sound", (int)ESOUND_STATE.ON));
        ApplyHandDirection((EHAND_DIRECTION)PlayerPrefs.GetInt("HandDirection", (int)EHAND_DIRECTION.RIGHT));
    }

    #region Touch On/Off

    public static ETOUCH_STATE TouchState;

    public static void ApplyTouchState(ETOUCH_STATE state)
    {
        TouchState = state;

        Managers.Input.touchState = state;
    }

    #endregion

    #region Sound On/Off

    public static ESOUND_STATE SoundState;

    public static void ApplySoundState(ESOUND_STATE state)
    {
        SoundState = state;

        Managers.Sound.SetVolume(state);
    }

    #endregion

    #region Hand Direction

    public static EHAND_DIRECTION HandDirection = EHAND_DIRECTION.RIGHT;

    public static void ApplyHandDirection(EHAND_DIRECTION direction)
    {
        HandDirection = direction;

        Managers.Point.SetHandDirection(direction);
    }

    #endregion
}
