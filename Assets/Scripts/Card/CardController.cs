using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardController : MonoBehaviour // 클릭, 드래그 함수 관리
{
	public enum InputType
	{
		IDLE,
		DRAG
	}

	public InputType _InputType = InputType.IDLE;

	private void Start()
	{
		GameManager._Input.InputAction -= InputMouseUpdate;
		GameManager._Input.InputAction += InputMouseUpdate;
	}

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
