using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardController : MonoBehaviour // 클릭, 드래그 함수 관리
{
	[SerializeField] private GameObject InputGameObject;
	public enum InputType
	{
		IDLE,
		DRAG
	}

	public InputType _InputType = InputType.IDLE;

	private void Start()
	{
		GameManager._Input.InputAction = InputTypeUpdate;
		GameManager._Input.InputAction += InputCardUpdate;
	}

	private void InputTypeUpdate()
	{
		if (Input.GetMouseButton(0))
			_InputType = InputType.DRAG;
		if (Input.GetMouseButtonUp(0))
			_InputType = InputType.IDLE;
	}

	private void InputCardUpdate()
	{
		InputGameObject = EventSystem.current.currentSelectedGameObject;
		if(InputGameObject == null || InputGameObject.CompareTag("Card") == false)
			return;
		Debug.Log("Input Card!");
	}

	public void Move()
    {

    }

    public void Drag()
	{

	}
}
