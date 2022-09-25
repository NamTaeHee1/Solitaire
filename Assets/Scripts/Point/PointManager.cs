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
			{
				PointManager _PointManager = Instantiate(Resources.Load<GameObject>("Prefabs/PointManager")).GetComponent<PointManager>();
				_instance = _PointManager;
				DontDestroyOnLoad(_instance);
			}
			return _instance; 
		}
	}

	private static PointManager _instance;

	public Point[] K;
	public Point[] A;
	public Point SelectCardPoint;

	private void Start()
	{
		GameObject K_Points = GameObject.Find("K_Points");
		K = new Point[K_Points.transform.childCount];

		for (int i = 0; i < K_Points.transform.childCount; i++)
		{
			K[i] = K_Points.transform.GetChild(i).GetComponent<Point>();
		}

		GameObject A_Points = GameObject.Find("A_Points");
		A = new Point[A_Points.transform.childCount];

		for (int i = 0; i < A_Points.transform.childCount; i++)
		{
			A[i] = A_Points.transform.GetChild(i).GetComponent<Point>();
		}

		SelectCardPoint = GameObject.Find("SelectCardPoint").GetComponent<Point>();
	}

}
