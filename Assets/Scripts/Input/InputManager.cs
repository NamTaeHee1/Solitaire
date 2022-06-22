using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
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

	public void OnDrag(PointerEventData eventData)
	{
		throw new System.NotImplementedException();
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log(eventData.ToString());
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		throw new System.NotImplementedException();
	}
}
