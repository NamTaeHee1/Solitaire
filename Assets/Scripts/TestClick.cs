using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TestClick : MonoBehaviour, IDragHandler, IPointerDownHandler
{
	public Vector2 Pos = Vector2.zero;

	public void OnDrag(PointerEventData eventData)
	{
		GetClickPos(eventData);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		GetClickPos(eventData);
	}

	private void Update()
	{
		Vector2 UIMousePos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GameObject.Find("Canvas").GetComponent<RectTransform>(), Camera.main.WorldToScreenPoint(Input.mousePosition), Camera.main, out Vector2 pos))
			UIMousePos = pos;
		Debug.Log($"MousePosition : {Input.mousePosition}, Pos : {Pos}, UI.MousePosition : {UIMousePos}, " +
			$"UI.Pos : {Camera.main.ScreenToWorldPoint(Pos)}, Image.AnchorPos : {GetComponent<RectTransform>().anchoredPosition}");
	}

	private void GetClickPos(PointerEventData eventData)
	{
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), // Card Image의 클릭한 부분을 가져오는 코드
		eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
			Pos = localCursor;
	}
}
