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
	[SerializeField] private Image cardImage;

	// 현재 카드 클릭 및 이동 가능한지 bool로 반환
	private Func<bool> cardInputBlock;

	public static float CHILD_CARD_POS = -50f;

	#region Card Propety, Init
	public string cardName 
	{
		get { return transform.name; }
		set { transform.name = value; }
	}

	public void SetCardInfo(Sprite CardFrontTexure, string CardName)
	{
		this.cardFrontTexture = CardFrontTexure;
		this.cardName = CardName;
	}

	private void SetCardState(ECardMoveState state) => cardState = state;
	#endregion

	#region Start
	private void Start()
	{
		cardInputBlock = () =>
		{
			return (cardTextureDirection == ECardDirection.BACK ||
						 cardState == ECardMoveState.MOVING);
		};
	}
	#endregion

	#region Texture
	private IEnumerator Show(ECardDirection _direction, float _waitTime = 0)
	{
		yield return new WaitForSeconds(_waitTime);

		Show(_direction);
	}

	public void Show(ECardDirection _direction)
	{
		switch (_direction)
		{
			case ECardDirection.FRONT:
				cardImage.sprite = cardFrontTexture;
				break;
			case ECardDirection.BACK:
				cardImage.sprite = cardBackTexture;
				break;
		}

		cardTextureDirection = _direction;
	}

	public void ShowCoroutine(ECardDirection _direction, float _waitTime = 0)
	{
		StartCoroutine(Show(_direction, _waitTime));
	}

	#endregion

	#region IHandler Functions
	[SerializeField] private bool isDrag = false;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (cardInputBlock())
			return;

		int pointChildCount = curPoint.GetChildCount() - 1;
		int cardSliblingIndex = transform.GetSiblingIndex();

		if(cardSliblingIndex != pointChildCount) // 현재 Point의 마지막 카드가 아니라면
		{
			for(int i = cardSliblingIndex + 1; i < pointChildCount; i++)
			{
				curPoint.transform.GetChild(i).SetParent(this.transform);
			}
		}

		transform.SetParent(GameSceneUI.Instance.cardCanvas.transform);
		transform.SetAsLastSibling();

		gameObject.GetOrAddComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		SetCardState(ECardMoveState.CLICKED);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (cardInputBlock())
			return;

		isDrag = true;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (cardInputBlock())
			return;

		SetCardState(ECardMoveState.DRAGING);

		Vector2 CardRectAnchorPos = cardRect.anchoredPosition;
		CardRectAnchorPos += eventData.delta;
		cardRect.anchoredPosition = CardRectAnchorPos;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (cardInputBlock())
			return;

		if (isDrag == false)
		{
			//TODO
		}
		isDrag = false;

		Move();
	}
	#endregion

	#region Move Function
	public void Move(Point _movePoint = null, float _waitTime = 0f)
	{
		// 플레이어가 드래그해서 PointerUp 함수가 호출 될 경우
		if (_movePoint == null)
		{
			List<Card> _overlapCards = SearchCardAround();

			if (_overlapCards.Count > 0)
			{
				Card _pCard = ChoicePCardFromList(_overlapCards);
				if (_pCard != null)
				{
					if(_pCard.curPoint != curPoint) // 이동하는 Point가 현재 Point와 다르다면 원래 Point의 마지막 카드를 앞면이 보이도록 수정
					{
						Card _pointLastCard = curPoint.GetLastCard();
						if (_pointLastCard != null)
							_pointLastCard.Show(ECardDirection.FRONT);
					}

					curPoint = _pCard.curPoint;
				}
			}

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
		transform.SetParent(_movePoint.transform);
		SetCardState(ECardMoveState.MOVING);
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
				SetCardState(ECardMoveState.IDLE);
				timer = 0f;
				break;
			}

			yield return null;
		}
	}

	#endregion

	#region pCard(Parent Card)
	/// <summary>
	/// overlapCards 리스트에서 조건에 알맞고 가장 가까운 카드를 반환
	/// </summary>
	/// <param name="overlapCards"></param>
	/// <returns></returns>
	private Card ChoicePCardFromList(List<Card> overlapCards)
	{
		foreach (Card card in overlapCards)
			Debug.Log($"CARD : {card}");
		for (int i = overlapCards.Count - 1; i >= 0; i--)
		{
			Card _card = overlapCards[i];
			if (_card.cardTextureDirection == ECardDirection.BACK) // 뒷면이면 pCard에서 제외
			{
				overlapCards.Remove(overlapCards[i]);
				continue;
			}
		}

		if (overlapCards.Count == 0) // 적합한 
			return null;

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