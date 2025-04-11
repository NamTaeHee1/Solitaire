using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    #region Init

    public void Awake()
    {
        retryButton.onClick.AddListener(RetryButtonClick);

        newGameButton.onClick.AddListener(NewGameButtonClick);
    }

    public void OnEnable()
    {
        Managers.Input.BlockingInput++;
    }

    public void OnDisable()
    {
        Managers.Input.BlockingInput--;
    }

    #endregion

    #region Retry

    [Header("다시하기 버튼")][SerializeField]
    private Button retryButton;

    public void RetryButtonClick()
    {
        Managers.Sound.Play("Press");

        Managers.Game.StartGame(true);

        gameObject.SetActive(false);
    }

    #endregion

    #region New Game

    [Header("새 게임 버튼")][SerializeField]
    private Button newGameButton;

    public void NewGameButtonClick()
    {
        Managers.Sound.Play("Press");

        Managers.Game.StartGame(false);

        gameObject.SetActive(false);
    }

    #endregion
}
