using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
	private static GameManager _instance = null;
	public static GameManager Instance
	{
		get 
		{
			if (_instance == null)
			{
				_instance = FindObjectOfType<GameManager>();
			}

			return _instance;
		}
	}

	public List<Card> deck = new List<Card>();
	public List<Card> deckInWaste = new List<Card>();

	private void Start()
	{
		GenerateCards();
		MoveCardToPoints();
	}

	private void GenerateCards()
	{
		Sprite[] cardFronts = Resources.LoadAll<Sprite>("Cards");
		Card card;

		GameObject cardPrefab = Resources.Load<GameObject>("Prefabs/Card");

		for (int i = 0; i < DEFINE.CARD_MAX_SIZE; i++)
		{
			card = Instantiate(cardPrefab, Managers.Point.stock.transform).GetComponent<Card>();
			card.SetCardInfo(cardFronts[i], cardFronts[i].name);

			SetCardPosZ(card.transform, -((i + 1) * 0.01f));

			deck.Add(card);
		}

		ShuffleDeck();

/*		int count = 0;
		while (IsSovable() == false)
		{
			ShuffleDeck();
			count++;
		}

		Debug.Log($"Count : {count}");*/
	}

	private const int MoveToTableauCardCount = 28;

	private bool IsSolvable()
	{
		/*int checkCount = 0, i = 0;

        #region Check K

        for(i = 0; i < MoveToTableauCardCount; i++)
        {
            if (deck[i].cardInfo.cardRank == ECardRank.K) checkCount++;
        }

        if (checkCount >= 2) return false;

        checkCount = 0;

        #endregion

        #region Check Ace

        for (i = 0; i < MoveToTableauCardCount; i++)
        {
            if (deck[i].cardInfo.cardRank == ECardRank.A) checkCount++;
        }

        if (checkCount == 4) return false;

        checkCount = 0;

        // Tableau에 이동 후 하단에 바로 맞출 수 있는 카드들이 한 세트라도 있는지 확인

        int tableauLastCardIndex = 0;
        int tableausLength = Managers.Point.tableaus.Length;
        int highNumCount = 0;
        int rankCount = 0;

        Card beforeCard = null, currentCard = null;

        for (i = 0; i < tableausLength - 1; i++)
        {
            beforeCard = deck[tableauLastCardIndex];

            tableauLastCardIndex += tableausLength - i;

            currentCard = deck[tableauLastCardIndex];

            if (beforeCard == null || currentCard == null) continue;

            int rankDistance = Mathf.Abs(beforeCard.cardInfo.cardRank - currentCard.cardInfo.cardRank);
            bool isSameColor = beforeCard.cardInfo.cardColor == currentCard.cardInfo.cardColor;

            if ((int)beforeCard.cardInfo.cardRank >= (int)ECardRank.NINE) highNumCount++;
            rankCount += (int)beforeCard.cardInfo.cardRank;

            if (rankDistance == 1 && isSameColor == false)
            {
                checkCount++;
            }
        }

        if (highNumCount < 2) return false;

        if (rankCount < 30) return false;

        if (checkCount == 0) return false;

        checkCount = highNumCount = rankCount = 0;

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
                string cardInfo = $"{card.cardInfo.cardRank}_{card.cardInfo.cardColor}";

                // 2 : Rank, 3 : Color
                moveTableauCards[j].Add(cardInfo);

                deckCopy.Remove(card);
            }
        }

        for (i = 0; i < tableausLength; i++)
        {
            // [0] : Rank, [1] : Color
            string[] cardInfo = moveTableauCards[i].First().Split('_');

            if (cardInfo[1].Equals("RED")) checkCount++;

            rankCount += (int)Enum.Parse(typeof(ECardRank), cardInfo[0]);

*//*            int cardsCount = moveTableauCards[i].Count;

            for (int j = 0; j < cardsCount; j++)
            {
                cardInfo = moveTableauCards[i].First().Split('_');

                moveTableauCards[i].Remove(moveTableauCards[i].First());

                if (moveTableauCards[i].Contains($"{cardInfo[0]}_{cardInfo[1]}"))
                    return false;

                if (moveTableauCards[i].Count == 0) break;
            }*//*
        }

        if (checkCount == tableausLength) return false;

        //if (rankCount < 45) return false;

        int lowNumCount = 0;

        for (i = 0; i < MoveToTableauCardCount; i++)
        {
            ECardRank rank = deck[i].cardInfo.cardRank;

            if (rank == ECardRank.A ||
                rank == ECardRank.TWO ||
                rank == ECardRank.THREE) lowNumCount++;

            if (rank == ECardRank.J ||
               rank == ECardRank.Q ||
               rank == ECardRank.K) highNumCount++;
        }

        if (lowNumCount > 10) return false;

        if (highNumCount < 4) return false;

        if (GetSuitableCardsCount(ECardColor.RED) < 11 &&
            GetSuitableCardsCount(ECardColor.BLACK) < 11) return false;

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

        if (GetSuitableColorCardsCount(ECardColor.RED) < (int)ECardRank.COUNT - 4 ||
            GetSuitableColorCardsCount(ECardColor.BLACK) < (int)ECardRank.COUNT - 4) return false;

        #endregion

        List<string> stockCardRanks = new List<string>();
        List<string> overlapCardSuits = new List<string>();

        for(i = MoveToTableauCardCount; i < deck.Count; i++)
        {
            string cardInfo = $"{deck[i].cardInfo.cardRank}_{deck[i].cardInfo.cardColor}";
            
            if(stockCardRanks.Contains(cardInfo) && overlapCardSuits.Contains(cardInfo) == false)
            {
                overlapCardSuits.Add(cardInfo);
            }

            stockCardRanks.Add(cardInfo);
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
*/
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

	private void ShuffleDeck()
	{
		for(int i = 0; i < deck.Count; i++)
		{
			int rand = Random.Range(0, deck.Count);

			Card temp = deck[i];
			deck[i] = deck[rand];
			deck[rand] = temp;
		}
	}

	private void SetCardPosZ(Transform cardTr, float z)
	{
		Vector3 cardPos = cardTr.localPosition;

		cardPos.z = z;

		cardTr.localPosition = cardPos;
	}

	private void MoveCardToPoints()
	{
		float waitTime = 0;
		Point[] tableaus = Managers.Point.tableaus;
		float zOffset = -0.1f;

		for (int i = 0; i < tableaus.Length; i++)
		{
			for (int j = i; j < tableaus.Length; j++)
			{
				Card card = deck.First<Card>();

				deck.Remove(card);

				Vector3 cardPos = card.transform.localPosition;
				cardPos.z = zOffset;

				card.transform.localPosition = cardPos;
				card.Move(tableaus[j], waitTime += 0.05f);
				card.ShowCoroutine(j == i ? ECardDirection.FRONT : ECardDirection.BACK, waitTime);

				zOffset -= 0.1f;
			}

			zOffset = -0.1f;
		}
	}
}
