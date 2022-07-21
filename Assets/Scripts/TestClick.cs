using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestClick : MonoBehaviour
{
	public RectTransform PanelUIRect;
	private Camera MainCam;
	public RectTransform ImageUIRect;

	private void Start()
	{
		MainCam = Camera.main;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(PanelUIRect, Input.mousePosition, MainCam, out Vector2 localPos);
			ImageUIRect.localPosition = localPos;
			ImageUIRect.localPosition += new Vector3(50, 50, 0);
		}
	}
}
