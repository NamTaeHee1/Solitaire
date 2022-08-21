using System;
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
	public CardEnum.CardDirection CardTextureDIrection = CardEnum.CardDirection.BACK;
	public Card pCard = null; // Parent Card

	[SerializeField] private Vector3 ChildCardPosition = Vector3.zero;
	[SerializeField] private RectTransform CardRect;

	// ��ȣ ���� ���� ex) ŷ, ��, ���̾Ƹ��

	// ���� ���� ���� ex) 1 ~ 9

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

		CardTextureDIrection = Direction;
	}
	#endregion

	#region IHandler Functions
	public void OnPointerDown(PointerEventData eventData)
	{
		gameObject.AddComponent<Rigidbody2D>();
		GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		SetCardState(CardEnum.CardState.CLICKED);
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (CardTextureDIrection == CardEnum.CardDirection.BACK)
			return;
		SetCardState(CardEnum.CardState.DRAGING);
		CardRect.anchoredPosition = Vector2.Lerp(CardRect.anchoredPosition, CardRect.anchoredPosition + eventData.delta, 1.0f);
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Destroy(gameObject.GetComponent<Rigidbody2D>());
		if (CardTextureDIrection == CardEnum.CardDirection.BACK)
			return;
		SetCardState(CardEnum.CardState.IDLE);
		Move();
	}
	#endregion

	#region Move & Drag Function
	public void Move(Point movePoint = null, float WaitTime = 0)
	{
		if(movePoint == null) // �÷��̾ �巡���ϰ� PointerUp �Լ��� ȣ�� �� ���
		{
			List<Card> OverlapCards = SearchCardAround();

			foreach (Card card in OverlapCards)
			{
				if (card.CardTextureDIrection == CardEnum.CardDirection.BACK // �޸��̰ų�
					/* ī�� ��Ģ�� ���� �ʰų� */)
					OverlapCards.Remove(card);
			}

			if (pCard != null)
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
			pCard = movePoint.transform.GetChild(transform.GetSiblingIndex() - 1).GetComponent<Card>();
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
			if (CardState == CardEnum.CardState.CLICKED)
				break;
			t += Time.deltaTime;
			CardRect.localPosition = Vector2.Lerp(CardRect.localPosition, ToPos, t / toPosTime);
			yield return null;
		}
	}

	private List<Card> SearchCardAround() // �ֺ� ī�� �˻� �� ����Ʈ�� ��ȯ & pCard�� �����ϴ� �Լ��� ���� ����
	{
		RectTransform CardCanvasRect = GameObject.Find("CardCanvas").GetComponent<RectTransform>();

		Vector2 CanvasSize = Camera.main.ScreenToWorldPoint(CardCanvasRect.sizeDelta) * 2;
		Vector2 CardSize = new Vector2(CanvasSize.x / (CardCanvasRect.sizeDelta.x / CardRect.sizeDelta.x),
															  CanvasSize.y / (CardCanvasRect.sizeDelta.y / CardRect.sizeDelta.y));

		Collider2D[] OverlapObjects = Physics2D.OverlapBoxAll(transform.position, CardSize, 0);

		List<Card> OverlapCards = new List<Card>();

		foreach (Collider2D Object in OverlapObjects)
		{
			if (Object.CompareTag("Card"))
				OverlapCards.Add(Object.GetComponent<Card>());
		}

		return OverlapCards;
	}
	#endregion
}