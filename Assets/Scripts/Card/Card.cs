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

	// ��ȣ ���� ���� ex) ŷ, ��, ���̾Ƹ��

	// ���� ���� ���� ex) 1 ~ 9

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

	// ī�尡 ��� ī��� ������ ���
	// 1. ī�� �̹����� ��� ī��� �̹����� ��ġ���� �������Ѵ� OnTriggerEnter, Stay�� ȣ���� �ʿ�
	// 2. ��� ī���� ����(�ڽ�)���� ������ ���� ī��� pCard ���ī��� cCard�� ������ϰ�,
	//     �ݴ�� �ٸ� ī���� �Ʒ���(�θ�)���� ������ ���� ī��� cCard ���ī��� pCard�� �������
	// 3. �ڽ� ������Ʈ�� �̵��ϴ°� �켱 ������ �з��� �Ⱥ��̱� ������ �ȵ�.
	// 4. Hierarchy �信 �ִ� ������ �̿��ؼ��� �����ؾ߸� ��
	// 5. ���� ī��� �� ī���� ��ġ���� ChildPosition�� Update�Լ����� ���������μ� ����

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