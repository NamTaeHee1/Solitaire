using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
	public static PointManager Instance
	{
		get { return _instance; }
	}

	private static PointManager _instance;

	private void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(this);
	}
}
