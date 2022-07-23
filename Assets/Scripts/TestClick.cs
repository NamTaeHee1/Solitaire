using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestClick : MonoBehaviour, IDragHandler
{
	public RectTransform ImageRect;
	public RectTransform CanvasRect;

	public void OnDrag(PointerEventData eventData)
	{
		ImageRect.anchoredPosition += eventData.delta;
	}
}
