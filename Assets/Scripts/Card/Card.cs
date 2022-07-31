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
		// 
		StartCoroutine(MoveCard(Vector3.zero, 0.5f)); // 일단 예시로 생성 지점으로 가게 설정
		
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
		// Trigger 확인 시 카드 밑으로 이동
	}
}