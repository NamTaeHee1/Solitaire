using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : MonoBehaviour
{
    [Header("손 방향 Toggles")][SerializeField]
    private Toggle[] handDirectionToggles;

    [Header("사운드 Toggles")][SerializeField]
    private Toggle[] soundToggles;

    [Header("터치 Toggles")][SerializeField]
    private Toggle[] touchToggles;

    [Header("돌아가기 버튼")][SerializeField]
    private Button resumeButton;

    #region Init

    private void Awake()
    {
        ApplyUIWithSavedData();

        handDirectionToggles[0].onValueChanged.AddListener((value) =>
        { ChangeHandDIrectionToggle(value, EHAND_DIRECTION.LEFT); });
        handDirectionToggles[1].onValueChanged.AddListener((value) =>
        { ChangeHandDIrectionToggle(value, EHAND_DIRECTION.RIGHT); });

        soundToggles[0].onValueChanged.AddListener((value) =>
        { ChangeSoundToggle(value, ESOUND_STATE.ON); });
        soundToggles[1].onValueChanged.AddListener((value) =>
        { ChangeSoundToggle(value, ESOUND_STATE.OFF); });

        touchToggles[0].onValueChanged.AddListener((value) =>
        { ChangeTouchToggle(value, ETOUCH_STATE.ON); });
        touchToggles[1].onValueChanged.AddListener((value) =>
        { ChangeTouchToggle(value, ETOUCH_STATE.OFF); });

        resumeButton.onClick.AddListener(ResumeButtonClick);
    }

    public void OnEnable()
    {
        Time.timeScale = 0f;

        Managers.Input.BlockingInput++;
    }

    public void OnDisable()
    {
        Time.timeScale = 1f;

        Managers.Input.BlockingInput--;
    }

    private void ApplyUIWithSavedData()
    {
        handDirectionToggles[0].isOn = false;
        handDirectionToggles[(int)Data.HandDirection].isOn = true;

        soundToggles[0].isOn = false;
        soundToggles[(int)Data.SoundState].isOn = true;

        touchToggles[0].isOn = false;
        touchToggles[(int)Data.TouchState].isOn = true;
    }

    #endregion

    #region Toggles

    public void ChangeHandDIrectionToggle(bool value, EHAND_DIRECTION direction)
    {
        if (value == false) return;

        Data.ApplyHandDirection(direction);

        PlayerPrefs.SetInt("HandDirection", (int)direction);
        PlayerPrefs.Save();
    }

    public void ChangeSoundToggle(bool value, ESOUND_STATE state)
    {
        if (value == false) return;

        Data.ApplySoundState(state);

        PlayerPrefs.SetInt("Sound", (int)state);
        PlayerPrefs.Save();
    }

    public void ChangeTouchToggle(bool value, ETOUCH_STATE state)
    {
        if (value == false) return;

        Data.ApplyTouchState(state);

        PlayerPrefs.SetInt("Touch", (int)state);
        PlayerPrefs.Save();
    }

    #endregion

    #region Resume

    private void ResumeButtonClick()
    {
        Managers.Sound.Play("Press");

        gameObject.SetActive(false);
    }

    #endregion
}
