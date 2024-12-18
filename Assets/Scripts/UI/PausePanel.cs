using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour
{
    #region Init

    private void Awake()
    {
        resumeButton.onClick.AddListener(ResumeButtonClick);

        retryButton.onClick.AddListener(RetryButtonClick);

        newGameButton.onClick.AddListener(NewGameButtonClick);
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

    #endregion

    #region Resume

    [Header("돌아가기 버튼")][SerializeField]
    private Button resumeButton;

    public void ResumeButtonClick()
    {
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        gameObject.SetActive(false);
    }

    #endregion

    #region Retry

    [Header("다시하기 버튼")][SerializeField]
    private Button retryButton;

    public void RetryButtonClick()
    {
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        Managers.Game.StartGame(true);
         
        gameObject.SetActive(false);
    }

    #endregion

    #region New Game

    [Header("새 게임 버튼")][SerializeField]
    private Button newGameButton;

    public void NewGameButtonClick()
    {
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        Managers.Game.StartGame(false);

        gameObject.SetActive(false);
    }

    #endregion
}