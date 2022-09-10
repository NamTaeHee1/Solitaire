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

	public Point[] K;
	public Point[] A;
	public Point SelectCardPoint;

	private void Awake()
	{
		_instance = this;
		DontDestroyOnLoad(this);

		GameObject K_Points = GameObject.Find("K_Points");
		Point[] K_Array = new Point[K_Points.transform.childCount];

		for (int i = 0; i < K_Points.transform.childCount; i++)
		{
			K[i] = K_Points.transform.GetChild(i).GetComponent<Point>();
		}

		GameObject A_Points = GameObject.Find("A_Points");
		Point[] A_Array = new Point[A_Points.transform.childCount];

		for (int i = 0; i < A_Points.transform.childCount; i++)
		{
			A[i] = A_Points.transform.GetChild(i).GetComponent<Point>();
		}

		SelectCardPoint = GameObject.Find("SelectCardPoint").GetComponent<Point>();
	}

}
