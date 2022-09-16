using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
	public int GetChildCount()
	{
		return transform.childCount;
	}

	public int GetPointFirstCardIdx()
	{
		for (int i = 0; i < GetChildCount() - 1; i++)
		{
			Card card = transform.GetChild(i).GetComponent<Card>();
			if (card.CardTextureDIrection == CardEnum.CardDirection.BACK)
				continue;
			else
				return i;
		}
		return GetChildCount() - 1;
	}

	public int GetPointLastCardIdx()
	{
		return GetChildCount() - 1;

	}

	public List<Card> GetMoveableCardList()
	{
		List<Card> CardList = new List<Card>();

		for (int i = GetPointFirstCardIdx(); i < GetPointLastCardIdx(); i++)
		{
			CardList.Add(transform.GetChild(i).GetComponent<Card>());
		}

		return CardList;
	}
}