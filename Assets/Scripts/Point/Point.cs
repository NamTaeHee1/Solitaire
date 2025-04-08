using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
	[Header("포인트에 카드가 쌓일 때 Y축이 점차 밑으로 내려가는가")]
	public bool useCardYOffset;

	[Header("Point 유형")]
	public EPOINT_TYPE pointType;

	public virtual Card GetLastCard()
	{
		int lastCardIndex = transform.childCount - 1;

		if (lastCardIndex < 0)
			return null;

		return transform.GetChild(lastCardIndex).GetComponent<Card>();
	}

	/// <summary>
	/// 현재 검색하고 있는 카드와 이 Point가 적합한가 (검색된 Point 시점으로 구현)
	/// </summary>
	public virtual bool IsSuitablePoint(Card card) { return false; }

    public virtual void OnEnterPoint(Card movedCard) { }

    public virtual void OnExitPoint(Card movedCard) { }
}
