using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
{
	[Header("ī�� Texture")]
	[SerializeField] private Sprite cardFrontTexture;
	[SerializeField] private Sprite cardBackTexture;

	[Header("ī�� Enum")]
	public CardEnum.ECardMoveState cardState = CardEnum.ECardMoveState.IDLE;
	public CardEnum.ECardDirection cardTextureDirection = CardEnum.ECardDirection.BACK;

	[Header("Parent Card")]
	public Card pCard = null; // Parent Card

	[Header("Point")]
	[SerializeField] private Point curPoint = null;
	[SerializeField] private Point prevPoint = null;

	[Header("Hierarchy���� ����")]
	[SerializeField] private Vector3 childCardPosition = Vector3.zero;
	[SerializeField] private RectTransform cardRect;

	#region Card Propety, Init
	public string cardName 
	{
		get { return cardName; }
		set { transform.name = value; }
	}

	public void SetCardInfo(Sprite CardFrontTexure, string CardName)
	{
		this.cardFrontTexture = CardFrontTexure;
		this.cardName = CardName;
	}

	private void SetCardState(CardEnum.ECardMoveState state) => cardState = state;
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
	public IEnumerator Show(CardEnum.ECardDirection Direction, float WaitTime = 0)
	{
		yield return new WaitForSeconds(WaitTime);

		Image CardImage = GetComponent<Image>();
		switch (Direction)
		{
			case CardEnum.ECardDirection.FRONT:
				CardImage.sprite = cardFrontTexture;
				break;
			case CardEnum.ECardDirection.BACK:
				CardImage.sprite = cardBackTexture;
				break;
		}

		cardTextureDirection = Direction;
	}
	#endregion

	#region Point

	private Point GetPoint()
	{
		return transform.parent.GetComponent<Point>();
	}
	#endregion

	#region IHandler Functions
	public void OnPointerDown(PointerEventData eventData)
	{
		if (cardTextureDirection == CardEnum.ECardDirection.BACK)
			return;
		gameObject.AddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		prevPoint = GetPoint();
		SetCardState(CardEnum.ECardMoveState.CLICKED);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (cardTextureDirection == CardEnum.ECardDirection.BACK)
			return;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (cardTextureDirection == CardEnum.ECardDirection.BACK)
			return;
		SetCardState(CardEnum.ECardMoveState.DRAGING);
		//CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1.0f);

		Vector3 CardRectAnchorPos = cardRect.anchoredPosition;
		CardRectAnchorPos.x += eventData.delta.x;
		CardRectAnchorPos.y += eventData.delta.y;
		cardRect.anchoredPosition = CardRectAnchorPos;

		curPoint = GetPoint();
		for (int i = 0; i < curPoint.GetChildCount(); i++)
			Debug.Log(curPoint.transform.GetChild(i).name);

	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (cardTextureDirection == CardEnum.ECardDirection.BACK)
			return;
		Destroy(gameObject.GetComponent<Rigidbody2D>());
		Move();
	}
	#endregion

	#region Move Function
	public void Move(Point movePoint = null, float WaitTime = 0)
	{
		// �÷��̾ �巡���ؼ� PointerUp �Լ��� ȣ�� �� ���
		if (movePoint == null)
		{
			List<Card> OverlapCards = SearchCardAround();
			if(OverlapCards.Count > 0)
				pCard = ChoosePCardFromList(OverlapCards);
			// pCard�� �ִٸ� PointUp�� ȣ��� ������ �ִ� ī��� �� pCard�� ������ ī�带 ã�� ��

			if(pCard == null)
				pCard = prevPoint.GetMoveableLastCard(); // �� ã�� ��� ���� ����Ʈ�� ������ ī��� pCard�� ����

			Point point = pCard.curPoint;

			if(point != prevPoint)
			{
				Card PrevPointLastCard = prevPoint.GetMoveableLastCard();
				PrevPointLastCard.StartCoroutine(PrevPointLastCard.Show(CardEnum.ECardDirection.FRONT));
			}

			curPoint = point;
			Move(point);

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
			pCard = movePoint.GetMoveableLastCard();
			transform.SetParent(pCard.transform);
			StartCoroutine(MoveCard(childCardPosition, WaitTime));
		}

		curPoint = movePoint;
	}

	private const float TO_POS_TIME = 0.75f;

	IEnumerator MoveCard(Vector3 ToPos, float WaitTime = 0)
	{
		float t = 0;

		cardState = CardEnum.ECardMoveState.MOVING;

		yield return new WaitForSeconds(WaitTime);

		while (TO_POS_TIME > t)
		{
			if (cardState == CardEnum.ECardMoveState.CLICKED)
				break;

			t += Time.deltaTime;
			cardRect.anchoredPosition = Vector2.Lerp(cardRect.anchoredPosition, ToPos, t / TO_POS_TIME);

			if (Vector3.Distance(cardRect.anchoredPosition, ToPos) < 0.1f) // ī�尡 ��ǥ������ ������ ���
			{
				cardState = CardEnum.ECardMoveState.IDLE;
				break;
			}

			yield return null;
		}
	}

	#endregion

	#region pCard(Parent Card)
	private Card ChoosePCardFromList(List<Card> OverlapCards) // ����Ʈ �߿��� ���� ������ ī��� ���� ����� ī�带 ��ȯ
	{
		for (int i = OverlapCards.Count - 1; i >= 0; i--)
		{
			if (OverlapCards[i].cardTextureDirection == CardEnum.ECardDirection.BACK) // �޸��̸� pCard���� ����
			{
				OverlapCards.Remove(OverlapCards[i]);
				continue;
			}

			List<Card> PointCardList = new List<Card>();
			PointCardList.AddRange(prevPoint.GetMoveableCardList());
			PointCardList.AddRange(curPoint.GetMoveableCardList());

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
		Vector2 CardSize = new Vector2(CanvasSize.x / (CardCanvasRect.sizeDelta.x / cardRect.sizeDelta.x),
															  CanvasSize.y / (CardCanvasRect.sizeDelta.y / cardRect.sizeDelta.y));

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
		if (cardState != CardEnum.ECardMoveState.DRAGING)
			return;
		RectTransform CardCanvasRect = GameObject.Find("CardCanvas").GetComponent<RectTransform>();

		Vector2 CanvasSize = Camera.main.ScreenToWorldPoint(CardCanvasRect.sizeDelta) * 2;
		Vector2 CardSize = new Vector2(CanvasSize.x / (CardCanvasRect.sizeDelta.x / cardRect.sizeDelta.x),
															  CanvasSize.y / (CardCanvasRect.sizeDelta.y / cardRect.sizeDelta.y));

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, CardSize);
	}
	#endregion
}