using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

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

		int count = 0;
		while (IsSovable() == false)
		{
			ShuffleDeck();
			count++;
		}

		Debug.Log($"Count : {count}");
	}

	private const int MoveToTableauCardCount = 28;

	private bool IsSovable()
	{
		int checkCount = 0, i;

		#region Check Color

		// 1. 카드의 색상이 4번 이상 겹치는지 확인

		bool isCurrentRed, isNextRed;

		for (i = 0; i < deck.Count - 1; i++)
		{
			isCurrentRed = deck[i].cardInfo.cardColor == ECardColor.RED;
			isNextRed = deck[i + 1].cardInfo.cardColor == ECardColor.RED;

			if (isCurrentRed && isNextRed) checkCount++;
			else checkCount = 0;
		}

		if (checkCount >= 4) return false;

		checkCount = 0;

		#endregion

		#region Check Rank

		// 2. Rank가 같은 카드가 2번 이상 겹치는지 확인

		ECardRank currentRank, nextRank;

		for (i = 0; i < deck.Count - 1; i++)
		{
			currentRank = deck[i].cardInfo.cardRank;
			nextRank = deck[i + 1].cardInfo.cardRank;

			if (currentRank == nextRank) checkCount++;
			else checkCount = 0;
		}

		if (checkCount >= 2) return false;

		checkCount = 0;

		#endregion

		#region Check Suit

		// 3. 무늬가 같은 카드가 3번 이상 겹치는지 확인

		ECardSuit currentSuit, nextSuit;

		for (i = 0; i < deck.Count - 1; i++)
		{
			currentSuit = deck[i].cardInfo.cardSuit;
			nextSuit = deck[i + 1].cardInfo.cardSuit;

			if (currentSuit == nextSuit) checkCount++;
			else checkCount = 0;
		}

		if (checkCount >= 3) return false;

		checkCount = 0;

		#endregion

		#region Check Ace

		// Tableau로 Ace 카드가 2장 이상 이동했는지 확인

		for (i = 0; i < MoveToTableauCardCount; i++)
		{
			if (deck[i].cardInfo.cardRank == ECardRank.A)
				checkCount++;
		}

		if (checkCount >= 2) return false;

		checkCount = 0;

		// Tableau에 이동 후 하단에 바로 맞출 수 있는 카드들이 한 세트라도 있는지 확인

		int tableausLength = Managers.Point.tableaus.Length;
		int tableauLastCardIndex = 0;

		Card beforeCard, currentCard;
		beforeCard = currentCard = null;

		for(i = 0; i < tableausLength - 1; i++)
		{
			beforeCard = deck[tableauLastCardIndex];

			tableauLastCardIndex += tableausLength - i;

			currentCard = deck[tableauLastCardIndex];

			if (beforeCard == null || currentCard == null) continue;

			int rankDistance = Mathf.Abs(beforeCard.cardInfo.cardRank - currentCard.cardInfo.cardRank);
			bool isSameColor = beforeCard.cardInfo.cardColor == currentCard.cardInfo.cardColor;

			if (rankDistance == 1 && isSameColor == false)
			{
				checkCount++;
			}
		}

		if (checkCount == 0) return false;

		checkCount = 0;

		#endregion

		#region Overlap Check

		int[] cardRankCounts = new int[(int)ECardRank.COUNT];

		for(i = 0; i < MoveToTableauCardCount; i++)
		{
			cardRankCounts[(int)deck[i].cardInfo.cardRank]++;
		}

		for(i = 0; i < cardRankCounts.Length; i++)
		{
			if (cardRankCounts[i] >= 4) return false;
		}

		#endregion

		return true;
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
			}

			zOffset -= 0.1f;
		}
	}
}
