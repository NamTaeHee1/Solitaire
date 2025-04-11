using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.U2D;

public class Stock : Point
{
	[Header("클릭 시 이동 Point")][SerializeField]
	private Point wastePoint;

	private Camera mainCam;

	private LayerMask stockLayer;

    // Card Pack Sheet
    public List<Sprite> cardSheet;

    private GameObject cardPrefab;

    private List<Card> Deck { get { return Managers.Game.deck; } }

    /// <summary>
    /// 다시하기 시 바뀌지 않은 현재 덱이 필요하기 때문
    /// </summary>
    private List<string> backUpCurrentDeck = new List<string>();

    [HideInInspector]
    public List<Card> allCards = new List<Card>();
    public Dictionary<string, Card> cardNameDict = new Dictionary<string, Card>();
    
	private void Awake()
	{
		mainCam = Camera.main;

		stockLayer = 1 << LayerMask.NameToLayer("Stock");

        cardPrefab = ResourcesCache<GameObject>.Load("Prefabs/Card");
    }

    #region Cards Settings

    public void GenerateCards(bool retryCurrentDeck)
    {
        int i;

        if (createdCard)
            ResetCardsSettings();
        else
            CreateCards();

        if (retryCurrentDeck)
        {
            for(i = 0; i < backUpCurrentDeck.Count; i++)
            {
                cardNameDict.TryGetValue(backUpCurrentDeck[i], out Card card);

                card.transform.SetAsLastSibling();
            }

            Deck.Clear();

            for(i = 0; i < transform.childCount; i++)
            {
                Deck.Add(transform.GetChild(i).GetComponent<Card>());
            }
        }
        else
        {
            ShuffleDeck();

            SolitaireSolver solver = new SolitaireSolver();

            while (solver.IsSolve(GetCardInfos()) == false)
            {
                ShuffleDeck();
            }

            backUpCurrentDeck.Clear();

            for (i = 0; i < Deck.Count; i++)
            {
                backUpCurrentDeck.Add(Deck[i].cardInfo.ToString());
            }
        }

        Transform cardTr;

        for (i = 0; i < Deck.Count; i++)
        {
            cardTr = Deck[i].transform;

            SetCardPosZ(cardTr, -((Deck.Count - i) * 0.01f));

            cardTr.SetAsFirstSibling();
        }

        /* 규칙성에 어긋날 시 재배치하는 코드 (현재 사용하지 않음)
        Dictionary<string, Sprite> frontsNameDict = new Dictionary<string, Sprite>();

        List<CardInfo> solvableDeck = solver.GetSolvableDeck(GetCardInfos());

        for (i = 0; i < cardFronts.Length; i++)
        {
            frontsNameDict.Add(cardFronts[i].name.ToLower(), cardFronts[i]);
        }

        for (int i = 0; i < solvableDeck.Count; i++)
        {
            CardInfo cardInfo = solvableDeck[i];
            string frontName = $"card_{cardInfo.cardSuit}_{cardInfo.cardRank}_{cardInfo.cardColor}";

            deck[i].SetCardInfo(frontsNameDict[frontName.ToLower()], frontName);
        }*/
    }

    private bool createdCard = false;

    private void CreateCards()
    {
        Card card;

        Sprite cardBack = cardSheet.Find(sprite => sprite.name.Equals("card_back"));

        for (int i = 0; i < cardSheet.Count; i++)
        {
            if (cardSheet[i] == cardBack) continue;

            card = Instantiate(cardPrefab, Managers.Point.stock.transform).GetComponent<Card>();

            card.SetCardInfo(cardSheet[i].name);
            card.SetCardTexture(cardSheet[i], cardBack);

            Deck.Add(card);

            cardNameDict.Add(card.cardInfo.ToString(), card);
        }

        allCards = new List<Card>(Deck);

        createdCard = true;
    }

    private List<string> GetCardInfos()
    {
        List<string> cardInfos = new List<string>();

        for (int i = 0; i < Deck.Count; i++)
        {
            cardInfos.Add(Deck[i].cardInfo.ToString());
        }

        return cardInfos;
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < Deck.Count; i++)
        {
            int rand = Random.Range(0, Deck.Count);

            Card temp = Deck[i];
            Deck[i] = Deck[rand];
            Deck[rand] = temp;
        }
    }

    private void SetCardPosZ(Transform cardTr, float z)
    {
        Vector3 cardPos = cardTr.localPosition;

        cardPos.z = z;

        cardTr.localPosition = cardPos;
    }

    public void MoveCardToPoints()
    {
        float waitTime = 0f, zOffset;
        Point[] tableaus = Managers.Point.tableaus;

        Managers.Input.BlockingInput++;
        Invoke(nameof(ActiveInput), 2f);

        Managers.Sound.Play("Shuffling");

        for (int i = 0; i < tableaus.Length; i++)
        {
            zOffset = -0.1f;

            for (int j = i; j < tableaus.Length; j++)
            {
                Card card = Deck.First();

                Deck.Remove(card);

                Vector3 cardPos = card.transform.localPosition;
                cardPos.z = zOffset;
                card.transform.localPosition = cardPos;

                card.Move(tableaus[j], waitTime += 0.05f, false);
                card.ShowCoroutine(i == j ? ECARD_DIRECTION.FRONT : ECARD_DIRECTION.BACK, waitTime);

                zOffset -= 0.1f;
            }
        }
    }

    private void ActiveInput()
    {
        Managers.Input.BlockingInput--;
    }

    public void ResetCardsSettings()
    {
        Managers.Game.deck.Clear();
        Managers.Game.deckInWaste.Clear();

        Transform cardTr;

        for (int i = 0; i < allCards.Count; i++)
        {
            cardTr = allCards[i].transform;

            cardTr.SetParent(transform);
            cardTr.localPosition = Vector3.zero;

            allCards[i].Show(ECARD_DIRECTION.BACK);

            Managers.Game.deck.Add(allCards[i]);
        }
    }

    /* 규칙성이 어긋날 시 재배치하는 코드 (현재 사용하지 않음)
    private List<CardInfo> GetCardInfos()
    {
        List<CardInfo> cardInfos = new List<CardInfo>();

        for (int i = 0; i < deck.Count; i++)
        {
            cardInfos.Add(deck[i].cardInfo);
        }

        return cardInfos;
    }

    private const int MoveToTableauCardCount = 28;

	private bool IsSolvable()
	{
		int checkCount = 0, i = 0;

        #region

        // Tableau에 이동 후 하단에 바로 맞출 수 있는 카드들이 한 세트라도 있는지 확인

        int tableauLastCardIndex = 0;
        int tableausLength = Managers.Point.tableaus.Length;

        List<Card> tableauLastCards = new List<Card>();

        for (i = 0; i < tableausLength; i++)
        {
            Card lastCard = deck[tableauLastCardIndex];

            if (lastCard == null) break;

            tableauLastCards.Add(lastCard);

            tableauLastCardIndex += tableausLength - i;
        }

        List<Card> alreayCheckedCards = new List<Card>();
        int rankDistance;
        bool isSameColor;

        for(i = 0; i < tableauLastCards.Count; i++)
        {
            Card currentCard = tableauLastCards[i];
            Card compareCard;

            for(int j = 1; j < tableauLastCards.Count; j++)
            {
                compareCard = tableauLastCards[j];

                if (compareCard == currentCard) continue;

                if (alreayCheckedCards.Contains(currentCard) ||
                    alreayCheckedCards.Contains(compareCard)) continue;

                if (checkCount == 2)
                {
                    if (currentCard.cardInfo.cardRank == compareCard.cardInfo.cardRank)
                    {
                        checkCount++;
                    }

                    continue;
                }

                rankDistance = Mathf.Abs(currentCard.cardInfo.cardRank - compareCard.cardInfo.cardRank);
                isSameColor = currentCard.cardInfo.cardColor == compareCard.cardInfo.cardColor;

                if (rankDistance == 1 && isSameColor == false)
                {
                    checkCount++;

                    alreayCheckedCards.Add(currentCard);
                    alreayCheckedCards.Add(compareCard);
                }
            }
        }

        if (checkCount != 3) return false;

        checkCount = 0;

        for (i = 0; i < tableausLength; i++)
        {
            ECardRank rank = tableauLastCards[i].cardInfo.cardRank;

            if (rank == ECardRank.A) checkCount++;
        }

        if (checkCount > 1) return false;

        checkCount = 0;

        #endregion

        #region Overlap Check

        // 같은 Tableau 내에 색상과 등급이 같은 카드가 공존하는지 확인

        List<string>[] moveTableauCards = new List<string>[tableausLength];
        List<Card> deckCopy = deck.ToList();

        for (i = 0; i < tableausLength; i++)
            moveTableauCards[i] = new List<string>();

        for (i = 0; i < tableausLength; i++)
        {
            for (int j = i; j < tableausLength; j++)
            {
                Card card = deckCopy.First();

                moveTableauCards[j].Add($"{card.cardInfo.cardRank}");

                deckCopy.Remove(card);
            }
        }

        for (i = 0; i < tableausLength; i++)
        {
            int cardsCount = moveTableauCards[i].Count;

            for (int j = 0; j < cardsCount; j++)
            {
                string rank = moveTableauCards[i].First();

                moveTableauCards[i].Remove(moveTableauCards[i].First());

                if (moveTableauCards[i].Contains(rank))
                    checkCount++;

                if (moveTableauCards[i].Count == 0) break;
            }
        }

        //if (checkCount == 0) return false;

        checkCount = 0;

        int lowNumCount = 0, highNumCount = 0;

        for (i = 0; i < MoveToTableauCardCount; i++)
        {
            ECardRank rank = deck[i].cardInfo.cardRank;

            if (rank <= ECardRank.THREE) lowNumCount++;

            if (rank >= ECardRank.J) highNumCount++;
        }

        if (lowNumCount > 10) return false;

        if (highNumCount < 4) return false;

        if (GetSuitableCardsCount(ECardColor.RED) <= 12 &&
            GetSuitableCardsCount(ECardColor.BLACK) <= 12) return false;

        checkCount = 0;

        int redCount = 0, blackCount = 0;

        for(i = 0; i < MoveToTableauCardCount; i++)
        {
            if (deck[i].cardInfo.cardColor == ECardColor.RED) redCount++;
            else blackCount++;
        }

        if (Mathf.Max(redCount, blackCount) - Mathf.Min(redCount, blackCount) > 4)
            return false;

        redCount = blackCount = 0;

        if (GetSuitableColorCardsCount(ECardColor.RED) < (int)ECardRank.COUNT - 3 ||
            GetSuitableColorCardsCount(ECardColor.BLACK) < (int)ECardRank.COUNT - 3) return false;

        #endregion

        List<string> stockCardRanks = new List<string>();
        List<string> overlapCardSuits = new List<string>();
        int[] cardSuitCounts = new int[(int)ECardSuit.COUNT];

        for (i = MoveToTableauCardCount; i < deck.Count; i++)
        {
            string cardInfo = $"{deck[i].cardInfo.cardRank}_{deck[i].cardInfo.cardColor}";

            if (stockCardRanks.Contains(cardInfo) && overlapCardSuits.Contains(cardInfo) == false)
            {
                overlapCardSuits.Add(cardInfo);
            }

            stockCardRanks.Add(cardInfo);

            cardSuitCounts[(int)deck[i].cardInfo.cardSuit]++;
        }

        if (overlapCardSuits.Count < 4) return false;

        for (i = 0; i < (int)ECardRank.COUNT; i++)
        {
            if (stockCardRanks.Contains($"{(ECardRank)i}_RED") ||
                stockCardRanks.Contains($"{(ECardRank)i}_BLACK")) checkCount++;
        }

        if (checkCount < (int)ECardRank.K) return false;

        redCount = blackCount = 0;

        for (i = 0; i < stockCardRanks.Count; i++)
            if (stockCardRanks[i].Contains("RED")) redCount++;

        blackCount = 24 - redCount;

        if (Mathf.Max(redCount, blackCount) - Mathf.Min(redCount, blackCount) > 6) return false;

        //if (cardSuitCounts.Min() < 4) return false;

        checkCount = highNumCount = 0;

        List<string> tableauFirstLineCards = new List<string>();

        for(i = 0; i < tableausLength; i++)
        {
            ECardRank rank = deck[i].cardInfo.cardRank;

            if (rank >= ECardRank.NINE) highNumCount++;

            if (tableauFirstLineCards.Contains($"{rank}")) continue;

            tableauFirstLineCards.Add($"{rank}");

            checkCount++;
        }

        //if (checkCount == tableausLength) return false;

        //if (highNumCount < 2) return false;

        checkCount = 0;

        int[] suitCounts = new int[(int)ECardSuit.COUNT];
        List<Card> lineCards = new List<Card>();

        for (i = 0; i < tableausLength; i++)
        {
            int j;
            lineCards = GetTableauLineCards(i, ELineAxis.Vertical);

            for (j = 0; j < lineCards.Count; j++)
                suitCounts[(int)lineCards[j].cardInfo.cardSuit]++;

            if (suitCounts.Max() - suitCounts.Min() > 2) return false;

            for (j = 0; j < suitCounts.Length; j++)
                suitCounts[j] = 0;

            lineCards.Clear();
        }

        for (i = 0; i < tableausLength; i++)
        {
            int j = 0;
            lineCards = GetTableauLineCards(i, ELineAxis.Horizontal);

            for (j = 0; j < lineCards.Count; j++)
                suitCounts[(int)lineCards[j].cardInfo.cardSuit]++;

            if (suitCounts.Max() - suitCounts.Min() > 3) return false;

            for (j = 0; j < suitCounts.Length; j++)
                suitCounts[j] = 0;

            lineCards.Clear();
        }

        for (i = 0; i < MoveToTableauCardCount; i++)
        {
            suitCounts[(int)deck[i].cardInfo.cardSuit]++;
        }

        //if (suitCounts.Max() - suitCounts.Min() != 4) return false;

        checkCount = 0;

        return true;
	}

    private int GetSuitableCardsCount(ECardColor aceColor)
    {
        List<string> moveTableauCardsToString = new List<string>();
        int checkCount = 0, i = 0;

        for (i = 0; i < MoveToTableauCardCount; i++)
            moveTableauCardsToString.Add($"{deck[i].cardInfo.cardRank}_{deck[i].cardInfo.cardColor}");

        for(i = 1; i < (int)ECardRank.COUNT; i++)
        {
            if (moveTableauCardsToString.Contains($"{(ECardRank)i}_{(ECardColor)(((int)aceColor + i - 1) % 2)}"))
                checkCount++;
        }

        return checkCount;
    }

    private int GetSuitableColorCardsCount(ECardColor cardColor)
    {
        int checkCount = 0, i = 0;
        ECardRank curRank = ECardRank.A;

        List<string> moveTableauCards = new List<string>();

        for (i = 0; i < MoveToTableauCardCount; i++)
        {
            moveTableauCards.Add($"{deck[i].cardInfo.cardRank}_{deck[i].cardInfo.cardColor}");
        }

        for(i = 0; i < (int)ECardRank.COUNT; i++)
        {
            if (moveTableauCards.Contains($"{curRank}_{cardColor}"))
                checkCount++;

            curRank++;
        }

        return checkCount;
    }

    private enum ELineAxis
    {
        Vertical,
        Horizontal
    }

    private List<Card> GetTableauLineCards(int index, ELineAxis axis)
    {
        List<Card> cards = new List<Card>();
        int indexCount;

        if (axis == ELineAxis.Vertical)
        {
            indexCount = index;

            for (int i = 0; i < index + 1; i++)
            {
                cards.Add(deck[indexCount]);

                indexCount += 7;
            }
        }
        else
        {
            indexCount = Managers.Point.tableaus.Length;
            int countNum = 0;

            for (int i = 0; i < index; i++)
                countNum += indexCount--;

            for (int i = countNum; i < countNum + indexCount; i++)
                cards.Add(deck[i]);
        }

        return cards;
    }

	
*/

    #endregion

    #region Touch Stock

    private void Update()
    {
        StockPointClick();
    }

    private void StockPointClick()
	{
        if (Managers.Input.BlockingInput != 0) return;

		if (Input.GetMouseButtonUp(0))
		{
            //if (drawTimer > 0f) return;

			RaycastHit2D hit = Utils.RaycastMousePos(mainCam, stockLayer);

			if (hit == false) return;

            if (Managers.Game.deck.Count        == 0 && 
                Managers.Game.deckInWaste.Count == 0) return;

            DrawCard();
		}
	}

    public void DrawCard()
    {
        DrawCardCommand command = new DrawCardCommand();

        command.Excute();

        Recorder.Push(command);
    }

    #endregion
}