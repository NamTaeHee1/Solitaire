using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTSCRIPT : MonoBehaviour
{
	public GameObject A;
	public GameObject B;

	private void Start()
	{
		Debug.Log(A.transform.childCount);
		int index = A.transform.childCount;
		for (int i = 0; i < index; i++)
			A.transform.GetChild(0).transform.SetParent(B.transform);
	}
}
