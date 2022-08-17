using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] private Sprite CardFrontTexture;
	[SerializeField] private Sprite CardBackTexture;
	public CardEnum.CardState CardState = CardEnum.CardState.IDLE;
	public CardEnum.CardDirection CardDIrection = CardEnum.CardDirection.BACK;
	public Card pCard = null; // Parent Card

	[SerializeField] private Vector3 ChildCardPosition = Vector3.zero;
	[SerializeField] private bool isTriggerOtherCard = false;

	// 기호 정보 변수 ex) 킹, 퀸, 다이아몬드

	// 숫자 정보 변수 ex) 1 ~ 9

	// 카드가 상대 카드와 만났을 경우
	// 1. 카드 이미지가 상대 카드와 이미지가 겹치도록 만나야한다 OnTriggerEnter, Stay에 호출이 필요
	// 2. 상대 카드의 윗면(자식)으로 가려면 현재 카드는 pCard 상대카드는 cCard가 없어야하고,
	//     반대로 다른 카드의 아랫면(부모)으로 가려면 현재 카드는 cCard 상대카드는 pCard가 없어야함
	// 3. 자식 오브젝트로 이동하는건 우선 순위가 밀려서 안보이기 때문에 안됨.
	// 4. Hierarchy 뷰에 있는 순서를 이용해서만 나열해야만 함
	// 5. 하위 카드는 위 카드의 위치에서 ChildPosition을 Update함수에서 더해줌으로서 고정


	#region Propety, Init
	public string CardName 
	{
		get { return CardName; }
		set { transform.name = value; }
	}

	public void SetCardInfo(Sprite CardFrontTexure, string CardName)
	{
		this.CardFrontTexture = CardFrontTexure;
		this.CardName = CardName;
	}

	private void SetCardState(CardEnum.CardState state) => CardState = state;
	#endregion

	#region Texture
	public void Show(CardEnum.CardDirection Direction)
	{
		switch (Direction)
		{
			case CardEnum.CardDirection.FRONT:
				GetComponent<Image>().sprite = CardFrontTexture;
				break;
			case CardEnum.CardDirection.BACK:
				GetComponent<Image>().sprite = CardBackTexture;
				break;
		}

		CardDIrection = Direction;
	}
	#endregion

	#region Point
	public Point GetCurPoint()
	{
		return transform.parent.GetComponent<Point>();
	}
	#endregion

	#region IHandler Functions
	public void OnPointerDown(PointerEventData eventData)
	{
		SetCardState(CardEnum.CardState.CLICKED);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (CardDIrection == CardEnum.CardDirection.BACK)
			return;
		SetCardState(CardEnum.CardState.DRAGING);
		CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1.0f);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		if (CardDIrection == CardEnum.CardDirection.BACK)
			return;
		SetCardState(CardEnum.CardState.IDLE);
		Move();
	}
	#endregion

	#region Move & Drag Function

	[SerializeField] private RectTransform CardRect;

	public void Move(Point movePoint = null, float WaitTime = 0, Card pCard = null)
	{
		if(movePoint == null) // 플레이어가 드래그하고 PointerUp 함수가 호출 될 경우
		{
			if (isTriggerOtherCard)
				StartCoroutine(MoveCard(pCard.transform.localPosition + ChildCardPosition, WaitTime));
			else
				StartCoroutine(MoveCard(Vector3.zero, WaitTime));

			return;
		}

		// 스크립트에서 Move 함수를 호출할 경우
		if (movePoint.GetChildCount() == 0) // 이동할 Point에 아무 카드도 없다면
		{
			transform.SetParent(movePoint.transform);
			StartCoroutine(MoveCard(Vector3.zero, WaitTime));
		}
		else // 있다면
		{ // pCard에 값을 넣어도 초기화됨
			transform.SetParent(movePoint.transform);
			pCard = movePoint.transform.GetChild(movePoint.GetChildCount() - 1).GetComponent<Card>();
			StartCoroutine(MoveCard(ChildCardPosition * (movePoint.GetChildCount() - 1), WaitTime));
		}
	}

	IEnumerator MoveCard(Vector3 ToPos, float WaitTime = 0)
	{
		float t = 0;
		float toPosTime = 0.75f;
		yield return new WaitForSeconds(WaitTime);
		while (toPosTime > t)
		{
			t += Time.deltaTime;
			CardRect.localPosition = Vector2.Lerp(CardRect.localPosition, ToPos, t / toPosTime);
			yield return null;
		}
	}
	#endregion

	#region OnTriggerEvents
	private void OnTriggerEnter2D(Collider2D collision)
	{
/*		if (CardState == CardEnum.CardState.IDLE || pCard != null)
			return;*/
		isTriggerOtherCard = true;
		//pCard = collision.GetComponent<Card>();
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
/*		if (collision.GetComponent<Card>() != null)
			pCard = collision.GetComponent<Card>();*/
		isTriggerOtherCard = true;
		//pCard = collision.GetComponent<Card>();
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		isTriggerOtherCard = false;
		//pCard = null;
	}
	#endregion
}