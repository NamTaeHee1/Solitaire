using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	public Sprite CardTexture;
	public State.CardState CardState = State.CardState.IDLE;
	public Card pCard = null; // Parent Card

	[SerializeField] private Vector3 ChildCardPosition = Vector3.zero;
	[SerializeField] private bool isTriggerOtherCard = false;

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
	}

	public void OnDrag(PointerEventData eventData)
	{
		SetCardState(State.CardState.DRAGING);
		CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1.0f);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		SetCardState(State.CardState.IDLE);
		Move();
	}
	#endregion

	#region Move & Drag Function

	[SerializeField] private RectTransform CardRect;

	public void Move(Point movePoint = null)
	{
		if(movePoint == null)
		{
			if (isTriggerOtherCard)
				StartCoroutine(MoveCard(pCard.transform.localPosition + ChildCardPosition));
			else
				StartCoroutine(MoveCard(Vector3.zero));

			return;
		}

		if (movePoint.GetChildCount() == 0)
		{
			transform.SetParent(movePoint.transform);
			StartCoroutine(MoveCard(Vector3.zero));
		}
		else
		{
			Card movePointLastCard = movePoint.transform.GetChild(movePoint.GetChildCount() - 1).GetComponent<Card>();
			transform.SetParent(movePoint.transform);
			StartCoroutine(MoveCard((movePoint.GetChildCount() - 1) * ChildCardPosition));
		}
	}

	IEnumerator MoveCard(Vector3 ToPos)
	{
		float t = 0;

		while (Vector3.Distance(CardRect.anchoredPosition, ToPos) > 0.01f)
		{
			t += Time.deltaTime * 0.5f;
			CardRect.localPosition = Vector3.Lerp(CardRect.localPosition, ToPos, t);
			yield return null;
		}
	}
	#endregion

	private void SetCardState(State.CardState state) => CardState = state;

	// 카드가 상대 카드와 만났을 경우
	// 1. 카드 이미지가 상대 카드와 이미지가 겹치도록 만나야한다 OnTriggerEnter, Stay에 호출이 필요
	// 2. 상대 카드의 윗면(자식)으로 가려면 현재 카드는 pCard 상대카드는 cCard가 없어야하고,
	//     반대로 다른 카드의 아랫면(부모)으로 가려면 현재 카드는 cCard 상대카드는 pCard가 없어야함
	// 3. 자식 오브젝트로 이동하는건 우선 순위가 밀려서 안보이기 때문에 안됨.
	// 4. Hierarchy 뷰에 있는 순서를 이용해서만 나열해야만 함
	// 5. 하위 카드는 위 카드의 위치에서 ChildPosition을 Update함수에서 더해줌으로서 고정

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (CardState == State.CardState.IDLE || pCard != null)
			return;
		isTriggerOtherCard = true;
		pCard = collision.GetComponent<Card>();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (CardState == State.CardState.IDLE || pCard != null)
			return;
		isTriggerOtherCard = true;
		pCard = collision.GetComponent<Card>();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		isTriggerOtherCard = false;
		pCard = null;
	}
} 