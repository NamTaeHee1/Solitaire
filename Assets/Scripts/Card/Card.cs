using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
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
}