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

	// ��ȣ ���� ���� ex) ŷ, ��, ���̾Ƹ��

	// ���� ���� ���� ex) 1 ~ 9

	// ī�尡 ��� ī��� ������ ���
	// 1. ī�� �̹����� ��� ī��� �̹����� ��ġ���� �������Ѵ� OnTriggerEnter, Stay�� ȣ���� �ʿ�
	// 2. ��� ī���� ����(�ڽ�)���� ������ ���� ī��� pCard ���ī��� cCard�� ������ϰ�,
	//     �ݴ�� �ٸ� ī���� �Ʒ���(�θ�)���� ������ ���� ī��� cCard ���ī��� pCard�� �������
	// 3. �ڽ� ������Ʈ�� �̵��ϴ°� �켱 ������ �з��� �Ⱥ��̱� ������ �ȵ�.
	// 4. Hierarchy �信 �ִ� ������ �̿��ؼ��� �����ؾ߸� ��
	// 5. ���� ī��� �� ī���� ��ġ���� ChildPosition�� Update�Լ����� ���������μ� ����


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
		if(movePoint == null) // �÷��̾ �巡���ϰ� PointerUp �Լ��� ȣ�� �� ���
		{
			if (isTriggerOtherCard)
				StartCoroutine(MoveCard(pCard.transform.localPosition + ChildCardPosition, WaitTime));
			else
				StartCoroutine(MoveCard(Vector3.zero, WaitTime));

			return;
		}

		// ��ũ��Ʈ���� Move �Լ��� ȣ���� ���
		if (movePoint.GetChildCount() == 0) // �̵��� Point�� �ƹ� ī�嵵 ���ٸ�
		{
			transform.SetParent(movePoint.transform);
			StartCoroutine(MoveCard(Vector3.zero, WaitTime));
		}
		else // �ִٸ�
		{ // pCard�� ���� �־ �ʱ�ȭ��
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