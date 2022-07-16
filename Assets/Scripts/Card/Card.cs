using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;

	public State.CardState CardState = State.CardState.IDLE;

	public Vector3 CardClickPos = Vector3.zero;

	public string CardName 
	{
		get { return CardName; }
		set { transform.name = value; }
	}

	public void SetCardInfo(Sprite CardTexure, string CardName)
	{
		this.CardTexture = CardTexure;
		this.CardName = CardName;
	}

	public void OnDrag(PointerEventData eventData)
	{
		transform.parent = GameObject.Find("UICanvas").transform;
		CardState = State.CardState.DRAGING;
		CardClickPos = GetClickedPos(eventData);
		SetCurrentInputCard(this);
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		transform.parent = GameObject.Find("UICanvas").transform;
		CardState = State.CardState.CLICKED;
		CardClickPos = GetClickedPos(eventData);
		SetCurrentInputCard(this);
	}

	private Vector2 GetClickedPos(PointerEventData eventData)
	{
		Vector2 ClickPos = Vector2.zero;
		if (RectTransformUtility.ScreenPointToLocalPointInRectangle(GetComponent<RectTransform>(), // Card Image의 클릭한 부분을 가져오는 코드
			eventData.position, eventData.pressEventCamera, out Vector2 localCursor))
				ClickPos = localCursor;

		Vector2 CardSize = GetComponent<RectTransform>().sizeDelta;
		Debug.Log($"CardClickPos.x : {(CardSize / 2 + ClickPos).x}, CardClickPos.y : {(CardSize / 2 + ClickPos).y}, CardSize : {CardSize}");
		return CardSize / 2 + ClickPos;
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		transform.parent.SetParent(GameObject.Find("CardSpawnPos").transform);
		CardState = State.CardState.IDLE;
		CardClickPos = Vector3.zero;
		SetCurrentInputCard(null);
	}

	private void SetCurrentInputCard(Card card)
	{
		CardManager.Instance.CurrentInputCard = card == null ? null : card;
	}
}