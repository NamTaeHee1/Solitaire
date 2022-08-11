using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance = null;
	public static GameManager Instance { get { return _instance; } }

	private static InputManager _input = new InputManager();
	static public InputManager _Input { get { return _input; } }

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else
			Destroy(this);
	}

	private void Update()
	{
		_Input.OnUpdate();
	}
}
