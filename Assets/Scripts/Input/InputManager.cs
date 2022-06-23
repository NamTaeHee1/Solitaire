using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
	public enum InputType
	{
		IDLE,
		DRAG
	}

	public InputType _InputType = InputType.IDLE;

	private void Update()
	{
		if (Input.GetMouseButton(0))
			_InputType = InputType.DRAG;
		if (Input.GetMouseButtonUp(0))
			_InputType = InputType.IDLE;

		print(_InputType);
	}
}
