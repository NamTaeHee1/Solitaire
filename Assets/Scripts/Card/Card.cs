using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;

	public enum CardState
	{  
		IDLE,
		CLICKED
	}

	public CardState _CardState = CardState.IDLE;

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
		throw new System.NotImplementedException();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("CardClicked");
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		throw new System.NotImplementedException();
	}
}