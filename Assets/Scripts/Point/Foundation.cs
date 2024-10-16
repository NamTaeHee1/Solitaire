using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Foundation : Point
{
	public override bool IsSuitablePoint(Card card)
	{
		// 1. 검색중인 카드에 자식이 있을 경우
		if (card.GetLastCard() != null) return false;

		// 2. 다른 Foundation에 이미 card와 같은 suit가 있다면
		if (IsSameSuitInOtherFoundations(card.cardInfo.cardSuit) == true) return false;

		Card childCard = GetLastCard();

		if(childCard == null)
		{
			if (card.cardInfo.cardRank != ECardRank.A) return false;
		}
		else
		{
			if (card.cardInfo.cardRank != childCard.cardInfo.cardRank + 1 ||
			    card.cardInfo.cardSuit  != childCard.cardInfo.cardSuit) return false;
		}

		return true;
	}

	private bool IsSameSuitInOtherFoundations(ECardSuit suit)
	{
		Point[] foundations = Managers.Point.foundations;

		for(int i = 0; i < foundations.Length; i++)
		{
			if (foundations[i] == this) continue;

			Card childCard = foundations[i].GetLastCard();

			if (childCard == null) continue;

            if (childCard.cardInfo.cardSuit == suit) return true;
		}

		return false;
	}
}
