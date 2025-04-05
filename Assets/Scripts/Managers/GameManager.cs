using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance = null;
	public static GameManager Instance
	{
		get 
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<GameManager>();
			}

			return _instance;
		}
	}

	public List<Card> deck = new List<Card>();
	public List<Card> deckInWaste = new List<Card>();

    #region Start Game

    private void Start()
    {
        StartGame(false);
    }

    public void StartGame(bool retryCurrentDeck)
    {
        GameUI.Instance.ResetUI();

        notAllowAutoComplete = false;

        Stock stock = Managers.Point.stock;

        stock.GenerateCards(retryCurrentDeck);
        stock.MoveCardToPoints();

        Recorder.Instance.ClearHistory();
    }

    #endregion

    #region Auto Complete

    /// <summary>
    /// 이번 판에서 자동 완성을 허락하지 않았을 경우(아니요 클릭)를 체크하기 위한 변수
    /// </summary>
    [HideInInspector]
    public bool notAllowAutoComplete = false;

    public void CheckAutoComplete()
    {
        if (Managers.Game.notAllowAutoComplete == true) return;

        List<Card> cards = Managers.Point.stock.allCards;

        for(int i = 0; i < cards.Count; i++)
        {
            Point cardPoint = cards[i].curPoint;

            if (cardPoint == null) continue;

            if (cardPoint.pointType != EPointType.TALBEAU &&
                cardPoint.pointType != EPointType.CARD) continue;

            // 현재 다른 카드 및 Tableau에 있는 카드 중 하나라도 뒷면이라면 자동 완성이 안됨
            if (cards[i].cardDirection == ECardDirection.BACK) return;
        }

        GameUI.Instance.ShowAutoCompletePopup();
    }

    #endregion

    #region Win

    public void CheckWin()
    {
        if (IsWin())
            StartCoroutine(GameWin());
    }

    public bool IsWin()
    {
        Point[] foundations = Managers.Point.foundations;
        Card lastCard;

        for (int i = 0; i < foundations.Length; i++)
        {
            lastCard = foundations[i].GetLastCard();

            if (lastCard == null) return false;

            if (lastCard.cardInfo.cardRank != ECardRank.K) return false;
        }

        return true;
    }

    WaitForSeconds wait1f = new WaitForSeconds(1.0f);

    private IEnumerator GameWin()
    {
        yield return wait1f;

        Managers.Input.BlockingInput--;

        GameUI.Instance.ShowWinPanel();
    }

    #endregion
}