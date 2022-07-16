using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;

	public State.CardState CardState = State.CardState.IDLE;

	public Vector3 CardClickPos = Vector3.zero;

	private RectTransform ChildRectTransform = null;

	private void Awake() => ChildRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

	public string CardName 
	{
		get { return CardName; }
		set { transform.name = value; }
	}

	public void SetCardInfo(Sprite CardTexure, string CardName)
	{
		this.CardTexture = CardTexure;
		this.CardName = CardName;
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		CardState = State.CardState.CLICKED;
		SetCurrentInputCard(this);
	}

	public void OnDrag(PointerEventData eventData)
	{
		CardState = State.CardState.DRAGING;
		SetClickedPos(eventData);
		SetCurrentInputCard(this);
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		SetClickedPos(eventData);
	}

	private void SetClickedPos(PointerEventData eventData)
	{
		if (CardClickPos != Vector3.zero)
			return;

		Vector2 ClickPos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), // Card Image의 클릭한 부분을 가져오는 코드
			eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
				ClickPos = localCursor;

		ChildRectTransform.anchoredPosition = -ClickPos;
		CardClickPos = ClickPos;
	}

	private void ResetClickedPos()
	{
		CardClickPos = Vector3.zero;
		ChildRectTransform.anchoredPosition = Vector2.zero;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		transform.parent = GameObject.Find("CardSpawnPos").transform;
		ResetClickedPos();
		CardState = State.CardState.IDLE;
		SetCurrentInputCard(null);
	}

	private void SetCurrentInputCard(Card card)
	{
		CardManager.Instance.CurrentInputCard = card == null ? null : card;
	}
}