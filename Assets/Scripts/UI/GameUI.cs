using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public static GameUI Instance
	{
		get
		{
			if(_instance == null)
				_instance = FindObjectOfType<GameUI>();
			return _instance;
		}
	}
	private static GameUI _instance;

    private void Awake()
    {
        pauseButton.onClick.AddListener(PauseButtonClick);
        hintButton.onClick.AddListener(HintButtonClick);
        undoButton.onClick.AddListener(UndoButtonClick);
        autoCompleteButton.onClick.AddListener(AutoCompleteButtonClick);

        hint = new HintSystem();
    }

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
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        if (Managers.Input.IsBlocking) return;

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

        Managers.Sound.Play(ESoundType.EFFECT, "Press");

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
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        if (Managers.Input.IsBlocking) return;

        Recorder recorder = Recorder.Instance;

        if (recorder.IsEmpty) return;

        recorder.Pop().Undo();
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
