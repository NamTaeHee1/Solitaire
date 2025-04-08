using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tableau : Point
{
	public override bool IsSuitablePoint(Card card)
	{
		// 1. card의 Rank가 K가 아니라면
		if (card.cardInfo.cardRank != ECARD_RANK.K) return false;

		// 2. 이미 다른 카드가 있을 경우
		if (GetLastCard() != null) return false;

		return true;
	}

    public override Card GetLastCard()
    {
        Card lastCard = base.GetLastCard();

        if (lastCard == null) return null;

        while(lastCard.transform.childCount != 0)
        {
            lastCard = lastCard.GetLastCard();
        }

        return lastCard;
    }

    #region Enter & Exit Point

    public override void OnEnterPoint(Card movedCard)
    {
        if (transform.childCount < 2) return;

        Card coverToCard = transform.GetChild(transform.childCount - 2).GetComponent<Card>();

        if (coverToCard != null)
            coverToCard.Show(ECARD_DIRECTION.BACK);
    }

    public override void OnExitPoint(Card movedCard)
    {
        Card pointLastCard = GetLastCard();

        if (pointLastCard != null)
            pointLastCard.Show(ECARD_DIRECTION.FRONT);
    }

    #endregion

}
