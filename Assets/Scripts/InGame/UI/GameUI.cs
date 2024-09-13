using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
	public static GameUI Instance
	{
		get
		{
			if(_instance == null)
				_instance = FindObjectOfType<GameUI>();
			return _instance;
		}
	}
	private static GameUI _instance;
}
