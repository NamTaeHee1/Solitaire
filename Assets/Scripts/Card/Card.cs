using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;

	public State.CardState CardState = State.CardState.IDLE;

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

	#region IHandler Functions
	public void OnPointerDown(PointerEventData eventData)
	{
		SetCardState(State.CardState.CLICKED);
		SetCurrentInputCard(this);
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetCardState(State.CardState.DRAGING);
		SetCurrentInputCard(this);
		Drag(eventData);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SetCardState(State.CardState.IDLE);
		SetCurrentInputCard(null);
		StartCoroutine(MoveCard());
	}
	#endregion

	#region Move & Drag Function

	[SerializeField] private RectTransform CardRect;

	private void Drag(PointerEventData eventData)
	{
		CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1f);
	}

	private void Move()
	{
		while (Vector3.Distance(CardRect.anchoredPosition, GameObject.Find("CardSpawner").transform.position) > 0.01f)
		{
			Debug.Log("몇번작동하는거야 도데체");
			CardRect.anchoredPosition = Vector3.Lerp(CardRect.localPosition, GameObject.Find("CardSpawner").transform.position, 0.5f);
		}
	}
	#endregion

	IEnumerator MoveCard()
	{
		float t = 0;

		while (Vector3.Distance(CardRect.anchoredPosition, GameObject.Find("CardSpawner").transform.position) > 0.01f)
		{
			t += Time.deltaTime;
			CardRect.anchoredPosition = Vector3.Lerp(CardRect.localPosition, GameObject.Find("CardSpawner").transform.position, t);
			yield return null;
		}
	}

	private void SetCardState(State.CardState state) => CardState = state;

	private void SetCurrentInputCard(Card card)
	{
		GameManager.Instance.CurrentInputCard = card == null ? null : card;
	}
}