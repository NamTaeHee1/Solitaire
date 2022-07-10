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

	public void OnDrag(PointerEventData eventData)
	{
		CardState = State.CardState.CLICKED;
		SetCurrentInputCard(this);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		CardState = State.CardState.CLICKED;
		SetCurrentInputCard(this);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		CardState = State.CardState.IDLE;
		SetCurrentInputCard(null);
	}

	private void SetCurrentInputCard(Card card)
	{
		CardManager.Instance.CurrentInputCard = card == null ? null : card;
	}
}