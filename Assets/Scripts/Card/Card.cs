using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;

	public State.CardState CardState = State.CardState.IDLE;

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
		SetCardState(State.CardState.CLICKED);
		SetCurrentInputCard(this);
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetCardState(State.CardState.DRAGING);
		Drag(eventData);
		SetCurrentInputCard(this);
	}

	private void Drag(PointerEventData eventData)
	{
		RectTransform CardRect = transform.GetComponent<RectTransform>();
		CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1f);
		Debug.Log(Time.deltaTime * 1000.0f);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SetCardState(State.CardState.IDLE);
		SetCurrentInputCard(null);
	}

	private void SetCardState(State.CardState state) => CardState = state;

	private void SetCurrentInputCard(Card card)
	{
		GameManager.Instance.CurrentInputCard = card == null ? null : card;
	}
}