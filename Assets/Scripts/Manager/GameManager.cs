using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	private static GameManager instance = null;
	public static GameManager Instance { get { return instance; } }

	private InputManager _input = new InputManager();
	static public InputManager _Input { get { return _Input; } }

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
		else
			Destroy(this);
	}
}
