using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Init

    private void Awake()
    {
        pauseButton.onClick.AddListener(PauseButtonClick);
        hintButton.onClick.AddListener(HintButtonClick);
        undoButton.onClick.AddListener(UndoButtonClick);
        settingButton.onClick.AddListener(SettingButtonClick);
        autoCompleteButton.onClick.AddListener(AutoCompleteButtonClick);
        collectionButton.onClick.AddListener(CollectionButtonClick);

        hint = new HintSystem();
    }

    #endregion

    #region Reset

    public void ResetUI()
    {
        autoCompleteButton.gameObject.SetActive(false);

        undoButton.gameObject.SetActive(true);

        hint.ClearLogs();
    }

    #endregion

    #region Pause

    [Header("일시 정지 버튼")][SerializeField]
    private Button pauseButton;

    [Header("일시 정지 Panel")]
    public PausePanel pausePanel;

    public void PauseButtonClick()
    {
        if (Managers.Input.IsBlocking) return;

        Managers.Sound.Play("Press");

        pausePanel.gameObject.SetActive(true);
    }

    #endregion

    #region Hint

    [Header("힌트 버튼")][SerializeField]
    private Button hintButton;

    [Header("힌트 버튼 쿨타임 이미지")][SerializeField]
    private Image hintButtonCool;

    private HintSystem hint;

    public void HintButtonClick()
    {
        if (hintButtonCool.gameObject.activeSelf) return;

        if (Managers.Input.IsBlocking) return;

        hint.ExploreAndMove();

        ActiveCooltime();
    }

    private void ActiveCooltime()
    {
        hintButtonCool.fillAmount = 1f;

        hintButtonCool.gameObject.SetActive(true);

        StartCoroutine(ExcuteCoolTimer());
    }

    private IEnumerator ExcuteCoolTimer()
    {
        while(hintButtonCool.fillAmount > 0f)
        {
            hintButtonCool.fillAmount -= Time.deltaTime * DEFINE.HINT_COOL * 0.1f;

            yield return null;
        }

        hintButtonCool.gameObject.SetActive(false);
    }

    #endregion

    #region Undo

    [Header("되돌리기 버튼")][SerializeField]
    private Button undoButton;

    public void UndoButtonClick()
    {
        if (Managers.Input.IsBlocking) return;

        if (Recorder.IsEmpty)
        {
            Managers.Sound.Play("Press");

            return;
        }

        Recorder.Pop().Undo();
    }

    #endregion

    #region Setting

    [Header("설정 버튼")][SerializeField]
    private Button settingButton;

    [Header("설정 Panel")][SerializeField]
    private SettingPanel settingPanel;

    public void SettingButtonClick()
    {
        if (Managers.Input.IsBlocking) return;

        Managers.Sound.Play("Press");

        settingPanel.gameObject.SetActive(true);
    }

    #endregion

    #region Collection

    [Header("콜렉션 버튼")][SerializeField]
    private Button collectionButton;

    [Header("콜렉션 Panel")][SerializeField]
    private CollectionPanel collectionPanel;

    private void CollectionButtonClick()
    {
        if (Managers.Input.IsBlocking) return;

        Managers.Sound.Play("Press");

        collectionPanel.gameObject.SetActive(true);
    }

    #endregion

    #region AutoComplete

    [Header("자동 완성 팝업")][SerializeField]
    private AutoCompletePopup autoCompletePopup;

    [Header("자동 완성 버튼")][SerializeField]
    private Button autoCompleteButton;

    public void ShowAutoCompletePopup()
    {
        autoCompletePopup.gameObject.SetActive(true);

        undoButton.gameObject.SetActive(false);
        autoCompleteButton.gameObject.SetActive(true);
    }

    public void AutoCompleteButtonClick()
    {
        Debug.Log($"fjewiofiwe");
        ShowAutoCompletePopup();

        autoCompletePopup.ConfirmButtonClick();
    }

    #endregion

    #region Win

    [Header("게임 클리어 Panel")][SerializeField]
    private WinPanel winPanel;

    public void ShowWinPanel()
    {
        winPanel.gameObject.SetActive(true);
    }

    #endregion
}
