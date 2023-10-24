using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSceneUI : MonoBehaviour
{
	public static GameSceneUI Instance
	{
		get
		{
			if(_instance == null)
				_instance = FindObjectOfType<GameSceneUI>();
			return _instance;
		}
	}
	private static GameSceneUI _instance;

	public Canvas cardCanvas;
}
