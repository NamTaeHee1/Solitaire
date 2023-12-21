using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType
{
	DUMMY,
	CARD,
	TABLEAU,
	WASTE,
	STOCK,
	FOUNDATION,
}

public class Point : MonoBehaviour
{
	public PointType pointType;

	public float childCardOffset = -50f;

	public int GetChildCount()
	{
		return transform.childCount;
	}

	public float GetDistance(Point _diffPoint)
	{
		Vector2 distance = transform.position - _diffPoint.transform.position;
		return distance.sqrMagnitude;
	}

	public Card GetLastCard()
	{
		int lastCardIndex = transform.childCount - 1;

		if (lastCardIndex < 0)
			return null;

		return transform.GetChild(lastCardIndex).GetComponent<Card>();
	}

	public Vector2 GetChildPos()
	{
		Vector2 cardOffset = new Vector2(0, transform.childCount * childCardOffset);
		return cardOffset;
	}
}
