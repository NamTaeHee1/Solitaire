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
	public ECardMoveState cardState = ECardMoveState.IDLE;
	public ECardDirection cardTextureDirection = ECardDirection.BACK;

	[Header("Point")]
	[SerializeField] private Point curPoint = null;

	[Header("Hierarchy에서 관리")]
	[SerializeField] private RectTransform cardRect;

	public static float CHILD_CARD_POS = -0.35f;

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

	private void SetCardState(ECardMoveState state) => cardState = state;
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
	private IEnumerator Show(ECardDirection _direction, float _waitTime = 0)
	{
		yield return new WaitForSeconds(_waitTime);

		Image CardImage = GetComponent<Image>();
		switch (_direction)
		{
			case ECardDirection.FRONT:
				CardImage.sprite = cardFrontTexture;
				break;
			case ECardDirection.BACK:
				CardImage.sprite = cardBackTexture;
				break;
		}

		cardTextureDirection = _direction;
	}

	public void ShowCoroutine(ECardDirection _direction, float _waitTime = 0)
	{
		StartCoroutine(Show(_direction, _waitTime));
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
		if (cardTextureDirection == ECardDirection.BACK)
			return;
		gameObject.GetOrAddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		SetCardState(ECardMoveState.CLICKED);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (cardTextureDirection == ECardDirection.BACK)
			return;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (cardTextureDirection == ECardDirection.BACK)
			return;
		SetCardState(ECardMoveState.DRAGING);
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
		if (cardTextureDirection == ECardDirection.BACK)
			return;
		Move();
	}
	#endregion

	#region Move Function
	public void Move(Point _movePoint = null, float _waitTime = 0f)
	{
		// 플레이어가 드래그해서 PointerUp 함수가 호출 될 경우
		if (_movePoint == null)
		{
			List<Card> OverlapCards = SearchCardAround();

			if (OverlapCards.Count > 0)
				curPoint = ChoicePCardFromList(OverlapCards).curPoint;

			Move(curPoint);

			return;
		}

		StartCoroutine(MoveCard(_movePoint, _waitTime));
	}

	private const float TO_POS_TIME = 0.75f;
	private float timer = 0f;
	private Vector2 toPos = Vector2.zero;

	IEnumerator MoveCard(Point _movePoint, float _waitTime = 0f)
	{
		cardState = ECardMoveState.MOVING;
		toPos = _movePoint.GetCardPos();

		yield return new WaitForSeconds(_waitTime);

		while (TO_POS_TIME > timer)
		{
			if (cardState == ECardMoveState.CLICKED)
				break;

			timer += Time.deltaTime;
			cardRect.anchoredPosition = Vector2.Lerp(cardRect.anchoredPosition, toPos, timer / TO_POS_TIME);

			if (Vector3.Distance(cardRect.anchoredPosition, toPos) < 0.1f) // 카드가 목표지점에 도착할 경우
			{
				curPoint = _movePoint;
				cardRect.anchoredPosition = toPos;
				cardState = ECardMoveState.IDLE;
				timer = 0f;
				break;
			}

			yield return null;
		}
	}

	#endregion

	#region pCard(Parent Card)
	private Card ChoicePCardFromList(List<Card> overlapCards) // 리스트 중에서 현재 선택한 카드와 가장 가까운 카드를 반환
	{
		for (int i = overlapCards.Count - 1; i >= 0; i--)
		{
			if (overlapCards[i].cardTextureDirection == ECardDirection.BACK) // 뒷면이면 pCard에서 제외
			{
				overlapCards.Remove(overlapCards[i]);
				continue;
			}

			List<Card> PointCardList = new List<Card>();
			PointCardList.AddRange(curPoint.GetMoveableCardList());

			foreach (Card card in PointCardList) // 저번 Point와 이번 Point의 카드는 pCard에서 제외
			{
				if (overlapCards[i] == card)
				{
					overlapCards.RemoveAt(i);
					continue;
				}
			}
		}

		if (overlapCards.Count == 0)
		{
			Debug.Log("OverlapCards.Count = 0");
			return null;
		}

		Card proximateCard = overlapCards[0];

		for (int i = 0; i < overlapCards.Count; i++)
		{
			Debug.Log($"{i}번째 카드 : {overlapCards[i].name}, 현재카드와의 Distance : {overlapCards[i].GetDistance(this)}");
		}

		if (overlapCards.Count > 1)
		{
			for (int i = 1; i < overlapCards.Count; i++)
			{
				if (overlapCards[i].GetDistance(this) < proximateCard.GetDistance(this))
					proximateCard = overlapCards[i];
			}
		}

		return proximateCard;
	}
	#endregion

	#region Interact with DifferentCard

	private float GetDistance(Card _diffCard)
	{
		Vector2 distance = transform.position - _diffCard.transform.position;
		return distance.sqrMagnitude;
	}

	private List<Card> SearchCardAround() // 자신을 제외한 주변 카드 검색 및 리스트로 반환 & pCard로 지정하는 함수는 따로 구현
	{
		RectTransform cardCanvasRect = GameObject.Find("CardCanvas").GetComponent<RectTransform>();

		Vector2 CanvasSize = Camera.main.ScreenToWorldPoint(cardCanvasRect.sizeDelta) * 2;
		Vector2 CardSize = new Vector2(CanvasSize.x / (cardCanvasRect.sizeDelta.x / cardRect.sizeDelta.x),
															  CanvasSize.y / (cardCanvasRect.sizeDelta.y / cardRect.sizeDelta.y));

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
		if (cardState != ECardMoveState.DRAGING)
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