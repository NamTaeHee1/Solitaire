using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public struct CardInfo
{
	public ECardSuit cardSuit;
	public ECardRank cardRank;
	public ECardColor cardColor;

    public CardInfo(ECardSuit suit, ECardRank rank, ECardColor color)
    {
        cardSuit = suit;
        cardRank = rank;
        cardColor = color;
    }

    public override bool Equals(object obj)
    {
        CardInfo info = (CardInfo)obj;

        if (cardSuit == info.cardSuit &&
            cardRank == info.cardRank) return true;

        return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"{cardSuit}_{cardRank}_{cardColor}";
    }
}

public class Card : Point
{
	[Header("카드 Texture")]
	[SerializeField] private Sprite cardFrontTexture;
	[SerializeField] private Sprite cardBackTexture;

	[Header("카드 상태")]
	public ECardMoveState cardState = ECardMoveState.IDLE;
	public ECardDirection cardDirection = ECardDirection.BACK;

	[Header("Point")]
	public Point curPoint;
	public Point beforePoint;

	[Header("Hierarchy에서 관리")]
	public SpriteRenderer cardSR;
	public Collider2D cardCollider;

    #region Card Info

    [Header("카드 정보")]
	public CardInfo cardInfo;

	public void SetCardInfo(Sprite cardFrontTexure, string cardName)
	{
		this.cardFrontTexture = cardFrontTexure;
		transform.name = cardName;

		string[] cardInfoArr = cardName.Split('_'); // [1] : Suit, [2] : Rank, [3] : Color
		cardInfo.cardSuit = (ECardSuit)Enum.Parse(typeof(ECardSuit), cardInfoArr[1].ToUpper());
		cardInfo.cardRank = (ECardRank)Enum.Parse(typeof(ECardRank), cardInfoArr[2].ToUpper());
		cardInfo.cardColor = (ECardColor)Enum.Parse(typeof(ECardColor), cardInfoArr[3].ToUpper());
	}

	private void SetCardState(ECardMoveState state) => cardState = state;

	#endregion

	#region Texture

	private IEnumerator Show(ECardDirection _direction, float _waitTime = 0)
	{
		yield return new WaitForSeconds(_waitTime);

		Show(_direction);
	}

	public void Show(ECardDirection _direction)
	{
		switch (_direction)
		{
			case ECardDirection.FRONT:
				cardSR.sprite = cardFrontTexture;
				break;
			case ECardDirection.BACK:
				cardSR.sprite = cardBackTexture;
				break;
		}

		cardDirection = _direction;
	}

	public void ShowCoroutine(ECardDirection _direction, float _waitTime = 0)
	{
		StartCoroutine(Show(_direction, _waitTime));
	}

    public void SetSortingOrder(int order)
    {
        cardSR.sortingOrder = order;
    }

	#endregion

	#region OnClick Functions

	public void OnClickDown()
	{
        SetCardState(ECardMoveState.CLICKED);

        List<Card> childCards = GetChildCards();

		for (int i = 0; i < childCards.Count; i++)
            childCards[i].SetSortingOrder(1);

        SetSortingOrder(1);
	}

	public void OnClicking(Vector3 mousePos)
	{
		SetCardState(ECardMoveState.DRAGING);

		mousePos.z = transform.position.z;

		transform.position = mousePos;
	}

	public void OnClickUp()
	{
        List<Point> overlapPoints = SearchPointAround();
        Point toPoint = ChoiceToPointFromList(overlapPoints);

        if (overlapPoints.Count == 0 || toPoint == null)
        {
            Move(curPoint, 0f, false);

            return;
        }

        MoveCardCommand command = new MoveCardCommand(this, curPoint, toPoint);

        command.Excute();

        Recorder.Instance.Push(command);
    }

    #endregion

    #region Move Function

    public void Move(Point movePoint, float waitTime = 0f, bool playSound = true)
    {
        transform.SetParent(movePoint.transform);

        beforePoint = curPoint;
        curPoint = movePoint;

        if (beforePoint != curPoint)
        {
            if (beforePoint != null) beforePoint.OnExitPoint(this);
            curPoint.OnEnterPoint(this);
        }

        Vector3 cardPos = transform.localPosition;

        cardPos.z = -((curPoint.transform.childCount + 1) * 0.1f);

        transform.localPosition = cardPos;

        Managers.Game.CheckAutoComplete();

        Managers.Game.CheckWin();

        if(playSound) Managers.Sound.Play(ESoundType.EFFECT, "MoveCard");

        StartCoroutine(MoveCard(movePoint, waitTime));
    }

	private IEnumerator MoveCard(Point movePoint, float _waitTime = 0f)
	{
		SetCardState(ECardMoveState.MOVING);

		float yOffset = -(movePoint.useCardYOffset ? DEFINE.CARD_CHILD_Y_OFFSET : 0f) *
					     (movePoint.transform.childCount - (movePoint is Card ? 0 : 1));

		Vector3 toPos = new Vector3(0, yOffset, transform.localPosition.z);

		yield return new WaitForSeconds(_waitTime);

		while (Vector2.Distance(transform.localPosition, toPos) > 0.01f)
		{
			if (cardState == ECardMoveState.CLICKED)
				yield break;

			transform.localPosition = Vector3.Lerp(transform.localPosition, 
											toPos, 
											Time.deltaTime * DEFINE.CARD_MOVE_SPEED);

			yield return null;
		}

		transform.localPosition = toPos;

		SetCardState(ECardMoveState.IDLE);

		List<Card> childCards = GetChildCards();

		for (int i = 0; i < childCards.Count; i++)
            childCards[i].SetSortingOrder(0);

        SetSortingOrder(0);
	}

	#endregion

	#region Child Card

	private List<Card> GetChildCards()
	{
		List<Card> childCards = new List<Card>();

		if (transform.childCount == 0) return childCards;

		Transform child = transform.GetChild(0);

		while(true)
		{
			childCards.Add(child.GetComponent<Card>());

			if  (child.childCount == 0) return childCards;
			else child = child.GetChild(0);
		}
	}

	#endregion

	#region Interact with DifferentCard

	private const string PointTag = "Point";

	private List<Point> SearchPointAround() // 자신을 제외한 주변 카드 검색 및 리스트로 반환 & pCard로 지정하는 함수는 따로 구현
	{
		Collider2D[] overlapObjects = Physics2D.OverlapBoxAll(transform.position, cardSR.size, 0);

		List<Point> overlapCards = new List<Point>();

		foreach (Collider2D obj in overlapObjects)
		{
			if (obj.CompareTag(PointTag))
				overlapCards.Add(obj.GetComponent<Point>());
		}

		return overlapCards;
	}

	private Point ChoiceToPointFromList(List<Point> overlapPoints)
	{
        for (int i = overlapPoints.Count - 1; i >= 0; i--)
        {
            if (overlapPoints[i].IsSuitablePoint(this) == false)
                overlapPoints.Remove(overlapPoints[i]);
        }

        if (overlapPoints.Count == 0) // 적합한 카드가 없다면
			return null;

		Point proximateCard = overlapPoints[0];

		if (overlapPoints.Count > 1)
		{
			for (int i = 1; i < overlapPoints.Count; i++)
			{
				if (GetDistance(overlapPoints[i]) < GetDistance(proximateCard))
					proximateCard = overlapPoints[i];
			}
		}

		return proximateCard;
	}

	public float GetDistance(Point _diffPoint)
	{
		Vector2 distance = transform.position - _diffPoint.transform.position;
		return distance.sqrMagnitude;
	}

	#endregion

	#region Check Suitable

	public override bool IsSuitablePoint(Card card)
	{
		// 1. 현재 내 카드의 CardRank가 한 단계 전이 아니라면
		if (card.cardInfo.cardRank + 1 != cardInfo.cardRank) return false;

		// 2. 다른색이 아니라면
		if (card.cardInfo.cardColor == cardInfo.cardColor) return false;

		// 3. 자식이 있다면 (하위 카드가 있다는 뜻)
		if (transform.childCount != 0) return false;

		// 4. 뒷면이라면
		if (cardDirection == ECardDirection.BACK) return false;

		// 5. Foundation 또는 Waste에 있다면
		if (curPoint.pointType == EPointType.WASTE ||
			curPoint.pointType == EPointType.FOUNDATION) return false;

		return true;
	}

	#endregion

	#region Gizmo
#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		if (cardState != ECardMoveState.DRAGING)
			return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, cardSR.size);
	}

#endif
    #endregion

}
