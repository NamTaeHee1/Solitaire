using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
	private Texture2D CardTexture { get; set; }
	private string CardName { get; set; }

	public void SetCardInfo(Texture2D CardTexure, string CardName)
	{
		this.CardTexture = CardTexure;
		this.CardName = CardName;
	}
}
