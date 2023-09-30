using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerUpHandler
{
	[Header("카드 Texture")]
	[SerializeField] private Sprite cardFrontTexture;
	[SerializeField] private Sprite cardBackTexture;

	[Header("카드 Enum")]
	public CardEnum.ECardMoveState cardState = CardEnum.ECardMoveState.IDLE;
	public CardEnum.ECardDirection cardTextureDirection = CardEnum.ECardDirection.BACK;

	[Header("Parent Card")]
	public Card pCard = null; // Parent Card

	[Header("Point")]
	[SerializeField] private Point curPoint = null;
	[SerializeField] private Point prevPoint = null;

	[Header("Hierarchy에서 관리")]
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
		// 플레이어가 드래그해서 PointerUp 함수가 호출 될 경우
		if (movePoint == null)
		{
			List<Card> OverlapCards = SearchCardAround();
			if(OverlapCards.Count > 0)
				pCard = ChoosePCardFromList(OverlapCards);
			// pCard가 있다면 PointUp이 호출된 시점에 있는 카드들 중 pCard로 적합한 카드를 찾은 뜻

			if(pCard == null)
				pCard = prevPoint.GetMoveableLastCard(); // 못 찾은 경우 저번 포인트의 마지막 카드로 pCard를 설정

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

		// 스크립트에서 Move 함수를 호출할 경우
		if (movePoint.GetChildCount() == 0) // 이동할 Point에 아무 카드도 없다면
		{
			transform.SetParent(movePoint.transform);
			StartCoroutine(MoveCard(Vector3.zero, WaitTime));
		}
		else // 있다면
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

			if (Vector3.Distance(cardRect.anchoredPosition, ToPos) < 0.1f) // 카드가 목표지점에 도착할 경우
			{
				cardState = CardEnum.ECardMoveState.IDLE;
				break;
			}

			yield return null;
		}
	}

	#endregion

	#region pCard(Parent Card)
	private Card ChoosePCardFromList(List<Card> OverlapCards) // 리스트 중에서 현재 선택한 카드와 가장 가까운 카드를 반환
	{
		for (int i = OverlapCards.Count - 1; i >= 0; i--)
		{
			if (OverlapCards[i].cardTextureDirection == CardEnum.ECardDirection.BACK) // 뒷면이면 pCard에서 제외
			{
				OverlapCards.Remove(OverlapCards[i]);
				continue;
			}

			List<Card> PointCardList = new List<Card>();
			PointCardList.AddRange(prevPoint.GetMoveableCardList());
			PointCardList.AddRange(curPoint.GetMoveableCardList());

			foreach (Card card in PointCardList) // 저번 Point와 이번 Point의 카드는 pCard에서 제외
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
			Debug.Log($"{i}번째 카드 : {OverlapCards[i].name}, 현재카드와의 Distance : {OverlapCards[i].GetDistance(this)}");
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

	private List<Card> SearchCardAround() // 자신을 제외한 주변 카드 검색 및 리스트로 반환 & pCard로 지정하는 함수는 따로 구현
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