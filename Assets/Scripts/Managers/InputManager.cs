using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum InputState
{
	IDLE,
	CLICKED,
	DRAGING
}

public class InputManager : MonoBehaviour
{
	public static InputManager Instance
	{
		get { return _instance; }
	}

	private static InputManager _instance;

	public InputState inputState = InputState.IDLE;

	public void SetState(InputState state)
	{
		inputState = state;
	}

	private void Awake() => _instance = this;
}
