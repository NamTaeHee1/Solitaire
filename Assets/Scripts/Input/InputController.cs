using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public static State.InputState _InputState = State.InputState.IDLE;

	private void Start()
	{
		GameManager._Input.InputAction = InputStateUpdate;
		GameManager._Input.InputAction += CardActionUpdate;
	}

	private void InputStateUpdate()
	{
		if (Input.GetMouseButton(0))
			_InputState = State.InputState.DRAG;
		if (Input.GetMouseButtonUp(0))
			_InputState = State.InputState.IDLE;
	}

	private void CardActionUpdate()
	{
		Card CurCard = CardManager.Instance.CurrentInputCard;
		if (CurCard == null)
			return;

		switch(CurCard.CardState)
		{
			case State.CardState.IDLE:
				CardManager.Instance.Move();
				Debug.Log("Move");
				break;
			case State.CardState.CLICKED:
				CardManager.Instance.Drag();
				Debug.Log("Drag");
				break;
		}
	}
}
