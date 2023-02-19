using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
	public int GetChildCount()
	{
		return transform.childCount;
	}

	public int GetMoveableFirstCardIdx()
	{
		for (int i = 0; i < GetChildCount() - 1; i++)
		{
			Card card = transform.GetChild(i).GetComponent<Card>();
			if (card.CardTextureDIrection == CardEnum.CardDirection.BACK)
				continue;
			else
				return i;
		}
		return GetChildCount();
	}

	public int GetMoveableLastCardIdx()
	{

		return GetChildCount();
	}

	public List<Card> GetMoveableCardList()
	{
		List<Card> CardList = new List<Card>();

		for (int i = GetMoveableFirstCardIdx(); i < GetMoveableLastCardIdx(); i++)
		{
			CardList.Add(transform.GetChild(i).GetComponent<Card>());
		}
		return CardList;
	}
}