using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
{
	[Header("ī�� Texture")]
	[SerializeField] private Sprite CardFrontTexture;
	[SerializeField] private Sprite CardBackTexture;

	[Header("ī�� Enum")]
	public CardEnum.CardState CardState = CardEnum.CardState.IDLE;
	public CardEnum.CardDirection CardTextureDIrection = CardEnum.CardDirection.BACK;

	[Header("Parent Card")]
	public Card pCard = null; // Parent Card

	[Header("Point")]
	[SerializeField] private Point CurPoint = null;
	[SerializeField] private Point PrevPoint = null;

	[Header("Hierarchy���� ����")]
	[SerializeField] private Vector3 ChildCardPosition = Vector3.zero;
	[SerializeField] private RectTransform CardRect;

	#region Card Propety, Init
	public string CardName 
	{
		get { return CardName; }
		set { transform.name = value; }
	}

	public void SetCardInfo(Sprite CardFrontTexure, string CardName)
	{
		this.CardFrontTexture = CardFrontTexure;
		this.CardName = CardName;
	}

	private void SetCardState(CardEnum.CardState state) => CardState = state;
	#endregion

/*	private void Update()
	{
		if (CardState == CardEnum.CardState.IDLE && pCard != null)
		{
			CurPoint = pCard.CurPoint;
			PrevPoint = pCard.PrevPoint;
			transform.localPosition = pCard.transform.localPosition + ChildCardPosition;
		}
	}*/

	#region Texture
	public IEnumerator Show(CardEnum.CardDirection Direction, float WaitTime = 0)
	{
		yield return new WaitForSeconds(WaitTime);

		Image CardImage = GetComponent<Image>();
		switch (Direction)
		{
			case CardEnum.CardDirection.FRONT:
				CardImage.sprite = CardFrontTexture;
				break;
			case CardEnum.CardDirection.BACK:
				CardImage.sprite = CardBackTexture;
				break;
		}

		CardTextureDIrection = Direction;
	}
	#endregion

	#region Point

	private void MovePoint(Point point)
	{
		CurPoint = GetPoint();
		int CardRectSiblingIndex = CurPoint.GetMoveableFirstCardIdx();
		int PointChildCount = CurPoint.GetMoveableLastCardIdx();

		if ((CardRectSiblingIndex == PointChildCount))
		{
			transform.SetParent(point.transform);
		}
		else
		{
			for (int i = CardRectSiblingIndex; i < PointChildCount; i++)
			{
				Card card = CurPoint.transform.GetChild(CardRectSiblingIndex).GetComponent<Card>();
				card.CardRect.SetParent(point.transform);
				card.transform.SetAsLastSibling();
			}
		}
	}

	private Point GetPoint()
	{
		return transform.parent.GetComponent<Point>();
	}
	#endregion

	#region IHandler Functions
	public void OnPointerDown(PointerEventData eventData)
	{
		if (CardTextureDIrection == CardEnum.CardDirection.BACK)
			return;
		gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		PrevPoint = GetPoint();
		SetCardState(CardEnum.CardState.CLICKED);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (CardTextureDIrection == CardEnum.CardDirection.BACK)
			return;
		MovePoint(PointManager.Instance.SelectCardPoint);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (CardTextureDIrection == CardEnum.CardDirection.BACK)
			return;
		SetCardState(CardEnum.CardState.DRAGING);
		//CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1.0f);

		Vector3 CardRectAnchorPos = CardRect.anchoredPosition;
		CardRectAnchorPos.x += eventData.delta.x;
		CardRectAnchorPos.y += eventData.delta.y;
		CardRect.anchoredPosition = CardRectAnchorPos;

		CurPoint = GetPoint();
		for (int i = 0; i < CurPoint.GetChildCount(); i++)
			Debug.Log(CurPoint.transform.GetChild(i).name);

	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (CardTextureDIrection == CardEnum.CardDirection.BACK)
			return;
		Destroy(gameObject.GetComponent<Rigidbody2D>());
		SetCardState(CardEnum.CardState.IDLE);
		Move();
	}
	#endregion

	#region Move Function
	public void Move(Point movePoint = null, float WaitTime = 0)
	{
		// �÷��̾ �巡���ϰ� PointerUp �Լ��� ȣ�� �� ���
		if (movePoint == null)
		{
			List<Card> OverlapCards = SearchCardAround();
			if(OverlapCards.Count > 0)
				pCard = ChoosePCardFromList(OverlapCards);
			// pCard�� �ִٸ� PointUp�� ȣ��� ������ �ִ� ī��� �� pCard�� ������ ī�带 ã�� ��

			Point point = (pCard == null) ? PrevPoint.GetComponent<Point>() : pCard.transform.parent.GetComponent<Point>();

			if (pCard == null) // ���� �ִ� Point�� �ٽ� �̵�
				StartCoroutine(MoveCard(ChildCardPosition * (point.GetChildCount() - 1), WaitTime));
			else // pCard�� �ִ� Point�� �̵�
			{
				CardRect.SetParent(pCard.transform);
				Move(point);

				Card PrevPointLastCard = PrevPoint.transform.GetChild(PrevPoint.GetMoveableLastCardIdx() - 1).GetComponent<Card>();
				PrevPointLastCard.StartCoroutine(PrevPointLastCard.Show(CardEnum.CardDirection.FRONT));
			}

			CurPoint = point;
			return;
		}

		// ��ũ��Ʈ���� Move �Լ��� ȣ���� ���
		if (movePoint.GetChildCount() == 0) // �̵��� Point�� �ƹ� ī�嵵 ���ٸ�
		{
			transform.SetParent(movePoint.transform);
			StartCoroutine(MoveCard(Vector3.zero, WaitTime));
		}
		else // �ִٸ�
		{
			transform.SetParent(movePoint.transform);
			pCard = movePoint.transform.GetChild(transform.GetSiblingIndex() - 1).GetComponent<Card>();
			StartCoroutine(MoveCard(ChildCardPosition * (movePoint.GetChildCount() - 1), WaitTime, (movePoint != PrevPoint)));
			//																										���� �� ����Ʈ�� ���� ����Ʈ�� �ٸ��ٸ� �ٸ� ī�� �ڽ����� ���°ŹǷ� GoToAnotherCardChild = true
		}
		CurPoint = movePoint;
	}

	IEnumerator MoveCard(Vector3 ToPos, float WaitTime = 0, bool GoToAnotherCardChild = false)
	{
		float t = 0;
		float toPosTime = 0.75f;
		CardState = CardEnum.CardState.MOVING;
		yield return new WaitForSeconds(WaitTime);
		while (toPosTime > t)
		{
			if (CardState == CardEnum.CardState.CLICKED)
				break;
			t += Time.deltaTime;
			CardRect.localPosition = Vector2.Lerp(CardRect.localPosition, ToPos, t / toPosTime);
			if (Vector3.Distance(CardRect.localPosition, ToPos) < 0.1f) // ī�尡 ��ǥ������ ������ ���
			{
				CardState = CardEnum.CardState.IDLE;
				if (GoToAnotherCardChild) // �ٸ� ī�� �ڽ����� ���ٸ� PCard�� ���󰡴� �Լ� ����
					StartCoroutine(FollowPCard());
				break;
			}
			yield return null;
		}
	}

	IEnumerator FollowPCard() // �� � ī�带 �巡���Ҷ� �� ī�� �ڽĵ��� �����ϵ��� ������
	{
		while (true)
		{
			if (AreAnyMovingPCard())
			{
				CardRect.localPosition = pCard.CardRect.localPosition + ChildCardPosition;
			}
			yield return null;
		}
	}

	private bool AreAnyMovingPCard()
	{
		Card _pCard = pCard;
		while (_pCard == null)
		{
			if (_pCard.CardState != CardEnum.CardState.IDLE)
				return true;

			_pCard = _pCard.pCard;
		}

		return false;
	}


	#endregion

	#region pCard(Parent Card)
	private Card ChoosePCardFromList(List<Card> OverlapCards) // ����Ʈ �߿��� ���� ������ ī��� ���� ����� ī�带 ��ȯ
	{
		for (int i = OverlapCards.Count - 1; i >= 0; i--)
		{
			if (OverlapCards[i].CardTextureDIrection == CardEnum.CardDirection.BACK) // �޸��̸� pCard���� ����
			{
				OverlapCards.Remove(OverlapCards[i]);
				continue;
			}

			List<Card> PointCardList = new List<Card>();
			PointCardList.AddRange(PrevPoint.GetMoveableCardList());
			PointCardList.AddRange(CurPoint.GetMoveableCardList());

			foreach (Card card in PointCardList) // ���� Point�� �̹� Point�� ī��� pCard���� ����
			{
				if (OverlapCards[i] == card)
				{
					OverlapCards.RemoveAt(i);
					continue;
				}
			}
		}

		if (OverlapCards.Count == 0)
		{
			Debug.Log("OverlapCards.Count = 0");
			return null;
		}

		Card ProximateCard = OverlapCards[0];

		for (int i = 0; i < OverlapCards.Count; i++)
		{
			Debug.Log($"{i}��° ī�� : {OverlapCards[i].name}, ����ī����� Distance : {OverlapCards[i].GetDistance(this)}");
		}

		if (OverlapCards.Count > 1)
		{
			for (int i = 1; i < OverlapCards.Count; i++)
			{
				if (OverlapCards[i].GetDistance(this) < ProximateCard.GetDistance(this))
					ProximateCard = OverlapCards[i];
			}
		}

		return ProximateCard;
	}
	#endregion

	#region Interact with DifferentCard

	private float GetDistance(Card DIffCard)
	{
		Vector2 distance = transform.position - DIffCard.transform.position;
		return distance.sqrMagnitude;
	}

	private List<Card> SearchCardAround() // �ڽ��� ������ �ֺ� ī�� �˻� �� ����Ʈ�� ��ȯ & pCard�� �����ϴ� �Լ��� ���� ����
	{
		RectTransform CardCanvasRect = GameObject.Find("CardCanvas").GetComponent<RectTransform>();

		Vector2 CanvasSize = Camera.main.ScreenToWorldPoint(CardCanvasRect.sizeDelta) * 2;
		Vector2 CardSize = new Vector2(CanvasSize.x / (CardCanvasRect.sizeDelta.x / CardRect.sizeDelta.x),
															  CanvasSize.y / (CardCanvasRect.sizeDelta.y / CardRect.sizeDelta.y));

		Collider2D[] OverlapObjects = Physics2D.OverlapBoxAll(transform.position, CardSize, 0);

		List<Card> OverlapCards = new List<Card>();

		foreach (Collider2D Object in OverlapObjects)
		{
			if (Object.CompareTag("Card") && Object.GetComponent<Card>() != this)
				OverlapCards.Add(Object.GetComponent<Card>());
		}

		return OverlapCards;
	}
	#endregion

	#region Gizmo
	private void OnDrawGizmos()
	{
		if (CardState != CardEnum.CardState.DRAGING)
			return;
		RectTransform CardCanvasRect = GameObject.Find("CardCanvas").GetComponent<RectTransform>();

		Vector2 CanvasSize = Camera.main.ScreenToWorldPoint(CardCanvasRect.sizeDelta) * 2;
		Vector2 CardSize = new Vector2(CanvasSize.x / (CardCanvasRect.sizeDelta.x / CardRect.sizeDelta.x),
															  CanvasSize.y / (CardCanvasRect.sizeDelta.y / CardRect.sizeDelta.y));

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, CardSize);
	}
	#endregion
}