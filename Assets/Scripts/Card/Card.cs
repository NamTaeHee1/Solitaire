using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	public Sprite CardTexture;

	public enum CardState
	{  
		IDLE,
		CLICK,
		DRAG
	}

	public CardState _CardState = CardState.IDLE;

	public string CardName 
	{
		get { return name; }
		set { transform.name = value;}
	}

	public void SetCardInfo(Sprite CardTexure, string CardName)
	{
		this.CardTexture = CardTexure;
		this.CardName = CardName;
	}

	public void Move()
	{
		if(_CardState == CardState.CLICK)
		{
			// Click Move Function
		}
		else if(_CardState == CardState.DRAG)
		{
			// Drag Move Function
		}
	}
}