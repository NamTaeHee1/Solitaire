using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
	public Action InputAction;

	public void OnUpdate()
	{
		if (InputAction != null)
			InputAction.Invoke();
	}
}
