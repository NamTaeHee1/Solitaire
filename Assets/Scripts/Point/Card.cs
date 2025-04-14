using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardInfo
{
	public ECARD_SUIT cardSuit;
	public ECARD_RANK cardRank;
	public ECARD_COLOR cardColor;

    public CardInfo(ECARD_SUIT suit, ECARD_RANK rank, ECARD_COLOR color)
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
	[SerializeField] private Sprite cardFront;
	[SerializeField] private Sprite cardBack;

	[Header("카드 상태")]
	public ECARD_MOVE_STATE cardState = ECARD_MOVE_STATE.IDLE;
	public ECARD_DIRECTION cardDirection = ECARD_DIRECTION.BACK;

	[Header("Point")]
	public Point curPoint;
	public Point beforePoint;

	[Header("Hierarchy에서 관리")]
	public SpriteRenderer cardSR;
	public Collider2D cardCollider;

    #region Card Info

    [Header("카드 정보")]
	public CardInfo cardInfo;

	public void SetCardInfo(string cardName)
	{
#if UNITY_EDITOR
        transform.name = cardName;
#endif

        cardInfo = cardName.ToCardInfo();
	}

    public void SetCardTexture(Sprite cardFront, Sprite cardBack)
    {
        this.cardFront = cardFront;
        this.cardBack = cardBack;

        Show(cardDirection);
    }

	private void SetCardState(ECARD_MOVE_STATE state) => cardState = state;

	#endregion

	#region Texture

	private IEnumerator Show(ECARD_DIRECTION _direction, float _waitTime = 0)
	{
		yield return new WaitForSeconds(_waitTime);

		Show(_direction);
	}

	public void Show(ECARD_DIRECTION _direction)
	{
		switch (_direction)
		{
			case ECARD_DIRECTION.FRONT:
				cardSR.sprite = cardFront;
				break;
			case ECARD_DIRECTION.BACK:
				cardSR.sprite = cardBack;
				break;
		}

		cardDirection = _direction;
	}

	public void ShowCoroutine(ECARD_DIRECTION _direction, float _waitTime = 0)
	{
		StartCoroutine(Show(_direction, _waitTime));
	}

    public void SetSortingOrder(int order)
    {
        cardSR.sortingOrder = order;
    }

    #endregion

    #region OnClick Functions

    private Vector2 clickPos; // 터치 감지 위해

	public void OnClickDown(Vector3 mousePos)
	{
        SetCardState(ECARD_MOVE_STATE.CLICKED);

        clickPos = mousePos;

        List<Card> childCards = GetChildCards();

		for (int i = 0; i < childCards.Count; i++)
            childCards[i].SetSortingOrder(1);

        SetSortingOrder(1);
	}

	public void OnClicking(Vector3 mousePos)
	{
		SetCardState(ECARD_MOVE_STATE.DRAGING);

        mousePos.z = transform.position.z;

		transform.position = mousePos;
	}

	public void OnClickUp(Vector3 mousePos)
	{
        List<Point> overlapPoints = SearchPointAround();
        Point toPoint = ChoiceToPointFromList(overlapPoints);

        if (Vector2.Distance(clickPos, mousePos) <= 0.1f && toPoint == null) // 터치일 경우
        {
            if (Managers.Input.touchState == ETOUCH_STATE.ON)
                toPoint = GetPointToMove();
        }

        if (toPoint == null)
        {
            Move(curPoint, 0f, false);

            return;
        }

        new MoveCardCommand(this, curPoint, toPoint).Excute();
    }

    private Point GetPointToMove()
    {
        #region Foundation

        Foundation foundationToMove = Managers.Point.foundations[(int)cardInfo.cardSuit];

        if (cardInfo.cardRank == ECARD_RANK.A)
            return foundationToMove;

        if (foundationToMove.GetLastCard() != null)
        {
            if (foundationToMove.IsSuitablePoint(this))
                return foundationToMove;
        }

        #endregion

        #region Tableau

        Tableau[] tableaus = Managers.Point.tableaus;

        for(int i = 0; i < tableaus.Length; i++)
        {
            if(cardInfo.cardRank == ECARD_RANK.K)
            {
                if (tableaus[i].GetLastCard() == null)
                    return tableaus[i];
            }
            else
            {
                Card lastCard = tableaus[i].GetLastCard();

                if (lastCard == null) continue;

                if (lastCard.IsSuitablePoint(this))
                    return lastCard;
            }
        }

        #endregion

        return null;
    }

    #endregion

    #region Move Function

    public void Move(Point movePoint, float waitTime = 0f, bool playSound = true)
    {
        transform.SetParent(movePoint.transform);

        Vector3 cardPos = transform.localPosition;

        cardPos.z = -((transform.GetSiblingIndex() + 1) * 0.1f);

        transform.localPosition = cardPos;

        beforePoint = curPoint;
        curPoint = movePoint;

        if (beforePoint != curPoint)
        {
            if (beforePoint != null) beforePoint.OnExitPoint(this);
            curPoint.OnEnterPoint(this);
        }

        Managers.Game.CheckAutoComplete();

        Managers.Game.CheckWin();

        if(playSound) Managers.Sound.Play("MoveCard");

        StartCoroutine(MoveCard(movePoint, waitTime));
    }

	private IEnumerator MoveCard(Point movePoint, float _waitTime = 0f)
	{
		SetCardState(ECARD_MOVE_STATE.MOVING);

		float yOffset = -(movePoint.useCardYOffset ? DEFINE.CARD_CHILD_Y_OFFSET : 0f) *
					     (movePoint.transform.childCount - (movePoint is Card ? 0 : 1));

		Vector3 toPos = new Vector3(0, yOffset, transform.localPosition.z);

		yield return new WaitForSeconds(_waitTime);

		while (Vector2.Distance(transform.localPosition, toPos) > 0.01f)
		{
			if (cardState == ECARD_MOVE_STATE.CLICKED)
				yield break;

			transform.localPosition = Vector3.Lerp(transform.localPosition, 
											toPos, 
											Time.deltaTime * DEFINE.CARD_MOVE_SPEED);

			yield return null;
		}

		transform.localPosition = toPos;

		SetCardState(ECARD_MOVE_STATE.IDLE);

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

    private int overlapLayer;
    
    private void Awake()
    {
        overlapLayer = LayerMask.NameToLayer("Point") & LayerMask.NameToLayer("Card");
    }

	private List<Point> SearchPointAround() // 자신을 제외한 주변 카드 검색 및 리스트로 반환
	{
		Collider2D[] overlapObjects = Physics2D.OverlapBoxAll(transform.position, 
                                                              cardSR.size, 
                                                              overlapLayer);
		List<Point> overlapPoints = new List<Point>();

        // Collider2D -> Point로 바꾸기 위해
        for(int i = 0; i < overlapObjects.Length; i++)
            overlapPoints.Add(overlapObjects[i].GetComponent<Point>());

		return overlapPoints;
	}

	private Point ChoiceToPointFromList(List<Point> overlapPoints)
	{
        // RemoveAt 메서드로 인한 인덱스 변화를 방지하기 위해 리스트를 역순으로 순회
        for (int i = overlapPoints.Count - 1; i >= 0; i--)
        {
            if (overlapPoints[i].IsSuitablePoint(this) == false)
                overlapPoints.RemoveAt(i);
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

	private float GetDistance(Point diffPoint)
	{
		Vector2 distance = transform.position - diffPoint.transform.position;
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
		if (cardDirection == ECARD_DIRECTION.BACK) return false;

		// 5. Foundation 또는 Waste에 있다면
		if (curPoint.pointType == EPOINT_TYPE.WASTE ||
			curPoint.pointType == EPOINT_TYPE.FOUNDATION) return false;

		return true;
	}

	#endregion

	#region Gizmo

#if UNITY_EDITOR

	private void OnDrawGizmos()
	{
		if (cardState != ECARD_MOVE_STATE.DRAGING)
			return;

		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(transform.position, cardSR.size);
	}

#endif

    #endregion

}
