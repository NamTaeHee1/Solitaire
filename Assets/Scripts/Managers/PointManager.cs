using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : MonoBehaviour
{
	public static PointManager Instance
	{
		 get 
		 {
			if (_instance == null)
				_instance = FindObjectOfType<PointManager>();

			return _instance; 
		 }
	}

	private static PointManager _instance;

	public Point[] tableaus;
	public Point[] foundations;
	public Point stock;

	public bool blockingRule = true;
}
