using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Foundation : Point
{
	public override bool IsSuitablePoint(Card card)
	{
		// 1. �˻����� ī�忡 �ڽ��� ���� ���
		if (card.GetLastCard() != null) return false;

		// 2. �ٸ� Foundation�� �̹� card�� ���� suit�� �ִٸ�
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
