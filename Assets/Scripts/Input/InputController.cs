using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
	public static State.InputState _InputState = State.InputState.IDLE;

	private void Start()
	{
		GameManager._Input.InputAction = InputStateUpdate;
	}

	private void InputStateUpdate()
	{
		if (Input.GetMouseButton(0))
			_InputState = State.InputState.DRAG;
		if (Input.GetMouseButtonUp(0))
			_InputState = State.InputState.IDLE;
	}
}
