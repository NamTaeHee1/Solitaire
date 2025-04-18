using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Data
{
    public static void Load()
    {
        ApplyTouchState((ETOUCH_STATE)PlayerPrefs.GetInt("Touch", (int)ETOUCH_STATE.ON));
        ApplySoundState((ESOUND_STATE)PlayerPrefs.GetInt("Sound", (int)ESOUND_STATE.ON));
        ApplyHandDirection((EHAND_DIRECTION)PlayerPrefs.GetInt("HandDirection", (int)EHAND_DIRECTION.RIGHT));
        ApplyCardPack((ECARD_PACK_TYPE)PlayerPrefs.GetInt("CardPack", (int)ECARD_PACK_TYPE.BASIC));
    }

    #region Touch On/Off

    

    public static void ApplyTouchState(ETOUCH_STATE state)
    {
        TouchState = state;

        Managers.Input.touchState = state;
    }

    #endregion
    public static ETOUCH_STATE TouchState;

    #region Sound On/Off



    public static void ApplySoundState(ESOUND_STATE state)
    {
        SoundState = state;

        Managers.Sound.SetVolume(state);
    }

    #endregion
    public static ESOUND_STATE SoundState;

    #region Hand Direction



    public static void ApplyHandDirection(EHAND_DIRECTION direction)
    {
        HandDirection = direction;

        Managers.Point.SetHandDirection(direction);
    }

    #endregion
    public static EHAND_DIRECTION HandDirection = EHAND_DIRECTION.RIGHT;

    #region Card Pack

    public static void ApplyCardPack(ECARD_PACK_TYPE type)
    {
        CardPack = type;
    }

    #endregion
    public static ECARD_PACK_TYPE CardPack;
}
