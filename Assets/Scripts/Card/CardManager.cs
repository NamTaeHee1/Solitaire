using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardManager : MonoBehaviour
{
	public Card CurrentInputCard = null;

	private static CardManager _Instance = null;
	public static CardManager Instance
	{
		get { return _Instance; }
	}

	private void Awake()
	{
		_Instance = this;
		DontDestroyOnLoad(this);
	}

	public void Move()
    {

    }

    public void Drag()
	{

	}

	public void ChangeState(Card Card, State.CardState Card_State, State.InputState Input_State)
	{
		Card.CardState = Card_State;
		InputController._InputState = Input_State;
	}
}