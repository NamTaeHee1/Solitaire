using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tableau : Point
{
	public override bool IsSuitablePoint(Card card)
	{
		// 1. card�� Rank�� K�� �ƴ϶��
		if (card.cardInfo.cardRank != ECardRank.K) return false;

		// 2. �̹� �ٸ� ī�尡 ���� ���
		if (GetLastCard() != null) return false;

		return true;
	}
}
