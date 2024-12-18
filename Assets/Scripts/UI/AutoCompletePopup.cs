using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoCompletePopup : MonoBehaviour
{
    [Header("Popup Visual")][SerializeField]
    private GameObject popupVisual;

    #region Init

    private void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmButtonClick);
        cancleButton.onClick.AddListener(CancleButtonClick);
    }

    private void OnEnable()
    {
        Managers.Input.BlockingInput++;
    }

    private void OnDisable()
    {
        popupVisual.SetActive(true);

        Managers.Input.BlockingInput--;
    }

    #endregion

    #region Confirm

    [Header("확인 버튼")][SerializeField]
    private Button confirmButton;

    public void ConfirmButtonClick()
    {
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        popupVisual.SetActive(false);

        StartCoroutine(ExcuteAutoComplete());
    }

    private WaitForSeconds wait01f = new WaitForSeconds(0.1f);

    private IEnumerator ExcuteAutoComplete()
    {
        GameManager Game = Managers.Game;
        Foundation[] foundations = Managers.Point.foundations;

        Managers.Input.BlockingInput++; // GameWin 함수에서 다시 --

        while(Game.IsWin() == false)
        {
            for(int i = 0; i < foundations.Length; i++)
            {
                Card lastCard = foundations[i].GetLastCard();

                if (lastCard == null) continue;

                CardInfo cardInfo = lastCard.cardInfo;

                if (cardInfo.cardRank == ECardRank.K) continue;

                string nextCardInfo = $"{cardInfo.cardSuit}_{cardInfo.cardRank + 1}_{cardInfo.cardColor}";

                Card nextCard = Utils.GetCard(nextCardInfo);

                if (nextCard.transform.childCount != 0) continue;

                nextCard.Show(ECardDirection.FRONT);
                nextCard.Move(foundations[i]);

                yield return wait01f;
            }
        }

        gameObject.SetActive(false);
    }

    #endregion

    #region Cancle

    [Header("취소 버튼")][SerializeField]
    private Button cancleButton;

    public void CancleButtonClick()
    {
        Managers.Sound.Play(ESoundType.EFFECT, "Press");

        gameObject.SetActive(false);
    }

    #endregion
}
