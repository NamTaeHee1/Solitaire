using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour // Ŭ��, �巡�� �Լ� ����
{
	public enum InputType
	{
		IDLE,
		DRAG
	}

	public InputType _InputType = InputType.IDLE;

	private void InputMouseUpdate()
	{
		if (Input.GetMouseButton(0))
			_InputType = InputType.DRAG;
		if (Input.GetMouseButtonUp(0))
			_InputType = InputType.IDLE;
	}

	public void Move()
    {

    }

    public void Drag()
	{

	}
}
