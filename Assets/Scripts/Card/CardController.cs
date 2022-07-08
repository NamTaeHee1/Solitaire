using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour // Ŭ��, �巡�� �Լ� ����
{
	[SerializeField] private GameObject InputGameObject;

	private State.InputState _InputState = State.InputState.IDLE;

	private void Start()
	{
		GameManager._Input.InputAction = InputTypeUpdate;
		GameManager._Input.InputAction += InputCardUpdate;
	}

	private void InputTypeUpdate()
	{
		if (Input.GetMouseButton(0))
			_InputState = State.InputState.DRAG;
		if (Input.GetMouseButtonUp(0))
			_InputState = State.InputState.IDLE;
	}

	private void InputCardUpdate()
	{
		Debug.Log("Input Card!");
	}

	public void Move()
    {

    }

    public void Drag()
	{

	}
}
