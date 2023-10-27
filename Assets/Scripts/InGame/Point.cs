using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType
{
	CARD,
	WASTE,
	STOCK,
	FOUNDATION,
}

public class Point : MonoBehaviour
{
	public PointType pointType;

	public int GetChildCount()
	{
		return transform.childCount;
	}

	public Card GetLastCard()
	{
		int lastCardIndex = GetChildCount() - 1;

		if (lastCardIndex < 0)
			return null;

		return transform.GetChild(lastCardIndex).GetComponent<Card>();
	}

	public Vector2 GetCardPos()
	{
		Vector2 cardOffset = new Vector2(0, transform.childCount * Card.CHILD_CARD_POS);
		return cardOffset;
	}
}
