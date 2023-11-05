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
	public static  InputManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<InputManager>();

			return _instance;
		}
	}

	private static  InputManager _instance;

	public InputState inputState = InputState.IDLE;
}
