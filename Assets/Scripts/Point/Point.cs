using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
	public int GetChildCount()
	{
		return transform.childCount;
	}

	public Card GetMoveableFirstCard()
	{
		if (GetChildCount() == 0)
			return null;

		Transform _curCard = transform;
		
		while(true)
		{
			Card _childCard = _curCard.GetChild(0).GetComponent<Card>();

			// 내 자식 오브젝트가 없거나 내 자식 카드 컴포넌트의 카드 방향이 앞면이라면 break;
			if (_childCard == null || _childCard.cardTextureDirection == CardEnum.ECardDirection.FRONT)
				break;
			else
				_curCard = _childCard.transform;
		}

		return _curCard.GetComponent<Card>();
	}

	public Card GetMoveableLastCard()
	{
		if (GetChildCount() == 0)
			return null;

		Transform _curCard = GetMoveableFirstCard().transform;

		while(true)
		{
			Card _childCard = _curCard.GetChild(0).GetComponent<Card>();

			//내 자식 오브젝트가 없으면 마지막이므로 break;
			if (_childCard == null)
				break;
			else
				_curCard = _childCard.transform;
		}

		return _curCard.GetComponent<Card>();
	}

	public List<Card> GetMoveableCardList()
	{
		List<Card> cardList = new List<Card>();
		Transform _curCard = GetMoveableFirstCard().transform;

		while(true)
		{
			cardList.Add(_curCard.GetComponent<Card>());

			if (_curCard.GetChild(0) == null)
				break;
			else
				_curCard = _curCard.GetChild(0);
		}

		return cardList;
	}
}