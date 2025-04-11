using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : SingletonMono<GameManager>
{
	public List<Card> deck = new List<Card>();
	public List<Card> deckInWaste = new List<Card>();

    #region Start Game

    public void StartGame(bool retryCurrentDeck)
    {
        Managers.UI.ResetUI();

        notAllowAutoComplete = false;

        Stock stock = Managers.Point.stock;

        stock.GenerateCards(retryCurrentDeck);
        stock.MoveCardToPoints();

        Recorder.ClearHistory();
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

            if (cardPoint.pointType != EPOINT_TYPE.TALBEAU &&
                cardPoint.pointType != EPOINT_TYPE.CARD) continue;

            // 현재 다른 카드 및 Tableau에 있는 카드 중 하나라도 뒷면이라면 자동 완성이 안됨
            if (cards[i].cardDirection == ECARD_DIRECTION.BACK) return;
        }

        Managers.UI.ShowAutoCompletePopup();
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

            if (lastCard.cardInfo.cardRank != ECARD_RANK.K) return false;
        }

        return true;
    }

    WaitForSeconds wait1f = new WaitForSeconds(1.0f);

    private IEnumerator GameWin()
    {
        yield return wait1f;

        Managers.Input.BlockingInput--;

        Managers.UI.ShowWinPanel();
    }

    #endregion

    #region Card Pack

    public void SetCardPack(List<Sprite> sheet)
    {
        Managers.Point.stock.cardSheet = sheet;
    }

    #endregion
}