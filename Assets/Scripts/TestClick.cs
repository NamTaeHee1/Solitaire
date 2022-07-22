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

	public Vector3 ClickVec = Vector3.zero;

	private void Start()
	{
		MainCam = Camera.main;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(ImageUIRect, Input.mousePosition, MainCam))
		{
			RectTransformUtility.ScreenPointToLocalPointInRectangle(PanelUIRect, Input.mousePosition, MainCam, out Vector2 localPos);
			if (ClickVec == Vector3.zero)
				ClickVec = localPos;
			ImageUIRect.localPosition = ImageUIRect.localPosition - new Vector3(ClickVec.x, ClickVec.y, 0);
			//ImageUIRect.localPosition = ((Vector3)localPos - ImageUIRect.localPosition);
		}
		if (Input.GetMouseButtonUp(0))
			ClickVec = Vector3.zero;
	}
}
