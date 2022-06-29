using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
	Action InputAction;

	public void OnUpdate()
	{
		if (!Input.anyKey)
			return;

		if (InputAction != null)
			InputAction.Invoke();
	}
}
