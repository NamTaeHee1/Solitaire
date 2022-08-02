using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;

	public State.CardState CardState = State.CardState.IDLE;

	public Card pCard = null; // Parent Card
	public Card cCard = null; // Child Card

	[SerializeField] private Vector3 ChildCardPosition = Vector3.zero;

	private bool isCheckTrigger = false;

	// 기호 정보 변수 ex) 킹, 퀸, 다이아몬드

	// 숫자 정보 변수 ex) 1 ~ 9

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
		Move();
	}
	#endregion

	#region Move & Drag Function

	[SerializeField] private RectTransform CardRect;

	private void Drag(PointerEventData eventData)
	{
		CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1.0f);
	}

	private void Move()
	{
		// 카드 위치 확인
		// 위치마다 분류 팔요
		if(isCheckTrigger)
			StartCoroutine(MoveCard(pCard.transform.position + ChildCardPosition, 0.5f));
		else
			StartCoroutine(MoveCard(Vector3.zero, 0.5f));

	}

	IEnumerator MoveCard(Vector3 ToPos, float Speed)
	{
		float t = 0;

		while (Vector3.Distance(CardRect.anchoredPosition, ToPos) > 0.01f)
		{
			t += Time.deltaTime * Speed;
			CardRect.anchoredPosition = Vector3.Lerp(CardRect.localPosition, ToPos, t);
			yield return null;
		}
	}
	#endregion

	private void SetCardState(State.CardState state) => CardState = state;

	private void SetCurrentInputCard(Card card)
	{
		GameManager.Instance.CurrentInputCard = card == null ? null : card;
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		isCheckTrigger = true;
		pCard = collision.GetComponent<Card>();
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		isCheckTrigger = false;
		pCard = null;
	}
} 