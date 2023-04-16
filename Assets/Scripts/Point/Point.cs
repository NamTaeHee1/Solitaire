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

		RectTransform _curCard = GetComponent<RectTransform>();
		
		while(true)
		{
			RectTransform _childCard;

			if (_curCard.GetChild(0) == null)
				break;
			else
				_childCard = _curCard.GetChild(0).GetComponent<RectTransform>();

			// �� �ڽ� ������Ʈ�� ���ų� �� �ڽ� ī�� ������Ʈ�� ī�� ������ �ո��̶�� break;
			if (_childCard.GetComponent<Card>() == null || _childCard.GetComponent<Card>().cardTextureDirection == CardEnum.ECardDirection.FRONT)
				break;
			else
				_curCard = _childCard;
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
			Transform _childCard = _curCard.GetChild(0);

			//�� �ڽ� ������Ʈ�� ������ �������̹Ƿ� break;
			if (_childCard == null)
				break;
			else
				_curCard = _childCard;
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