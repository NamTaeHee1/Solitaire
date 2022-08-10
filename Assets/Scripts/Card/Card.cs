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

	private bool isTriggerOtherCard = false;

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
		if(isTriggerOtherCard)
			StartCoroutine(MoveCard(ChildCardPosition, 0.5f));
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

	// 카드가 상대 카드와 만났을 경우
	// 1. 카드 이미지가 상대 카드와 이미지가 겹치도록 만나야한다 OnTriggerEnter, Stay에 호출이 필요
	// 2. 상대 카드의 윗면(자식)으로 가려면 현재 카드는 pCard 상대카드는 cCard가 없어야하고,
	//     반대로 다른 카드의 아랫면(부모)으로 가려면 현재 카드는 cCard 상대카드는 pCard가 없어야함
	// 3. 

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CardState == State.CardState.IDLE || pCard != null || cCard != null)
			return;
		isTriggerOtherCard = true;
		pCard = collision.GetComponent<Card>();
		collision.GetComponent<Card>().cCard = this;
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (CardState == State.CardState.IDLE || pCard != null || cCard != null)
			return;
		isTriggerOtherCard = true;
		pCard = collision.GetComponent<Card>();
		collision.GetComponent<Card>().cCard = this;
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		isTriggerOtherCard = false;
		pCard = null;
		collision.GetComponent<Card>().cCard = null;
	}
} 