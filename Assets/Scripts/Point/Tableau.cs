using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tableau : Point
{
	public override bool IsSuitablePoint(Card card)
	{
		// 1. card의 Rank가 K가 아니라면
		if (card.cardInfo.cardRank != ECardRank.K) return false;

		// 2. 이미 다른 카드가 있을 경우
		if (GetLastCard() != null) return false;

		return true;
	}
}
