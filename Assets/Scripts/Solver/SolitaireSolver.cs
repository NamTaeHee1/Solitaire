using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;
using UnityEngine;

public class SolitaireSolver
{
    private struct CardState
    {
        public string cardName;
        public bool isFaceUp;
        public bool movedDiffTableau;

        public CardState(string _cardName, bool _isFaceUp)
        {
            cardName = _cardName;
            isFaceUp = _isFaceUp;
            movedDiffTableau = false;
        }

        public override bool Equals(object obj)
        {
            return ((CardState)obj).cardName.Equals(this.cardName);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    private List<CardState>[] tableaus =
    {
        new List<CardState>(),
        new List<CardState>(),
        new List<CardState>(),
        new List<CardState>(),
        new List<CardState>(),
        new List<CardState>(),
        new List<CardState>()
    };
    private List<CardState> stock = new List<CardState>();
    private int[] foundations = { 0, 0, 0, 0 };

    private int RemainCards
    {
        get
        {
            return DEFINE.CARD_MAX_SIZE - (foundations[0] + foundations[1] +
                                           foundations[2] + foundations[3]);
        }
    }

    public bool IsSolve(List<string> deckToString)
    {
        Init(deckToString);

        return PlayGame();
    }

    private void Init(List<string> deckToString)
    {
        List<CardState> deck = new List<CardState>();

        CardState firstCard;
        int i;

        for(i = 0; i < deckToString.Count; i++)
        {
            deck.Add(new CardState(deckToString[i], false));
        }

        for (i = 0; i < tableaus.Length; i++)
            tableaus[i].Clear();

        for (i = 0; i < foundations.Length; i++)
            foundations[i] = 0;

        stock.Clear();

        for (i = 0; i < tableaus.Length; i++)
        {
            for (int j = i; j < tableaus.Length; j++)
            {
                firstCard = deck.First();

                deck.Remove(firstCard);

                if (i == j) firstCard.isFaceUp = true;

                tableaus[j].Add(firstCard);
            }
        }

        while (deck.Count != 0)
        {
            firstCard = deck.First();

            deck.Remove(firstCard);

            stock.Add(firstCard);
        }
    }

    private List<string> logs = new List<string>();
    private int moveCount = 0;

    private bool PlayGame()
    {
        int i;
        Random rand = new Random();

        while (RemainCards > 0)
        {
            for (i = 0; i < tableaus.Length; i++)
            {
                if (tableaus[i].Count == 0) continue;

                MoveFromTableauToFoundation(i);

                MoveFromTableauToDiffTableau(i);
            }

            MoveFromStockToDiffPoints();

            if (moveCount == 0)
            {
                logs.Clear();
                return false;
            }

            moveCount = 0;
        }

        for(i = 0; i < logs.Count; i++)
            Debug.Log($"logs [{i + 1}] : {logs[i]}");

        return true;
    }

    private ECardColor GetCardColor(ECardSuit suit)
    {
        if (suit == ECardSuit.DIAMONDS || suit == ECardSuit.HEARTS)
            return ECardColor.RED;
        else
            return ECardColor.BLACK;
    }

    private CardInfo GetCardInfo(string cardInfoStr)
    {
        string[] infos = cardInfoStr.Split('_');
        
        return new CardInfo((ECardSuit) Enum.Parse(typeof(ECardSuit),  infos[0]),
                            (ECardRank) Enum.Parse(typeof(ECardRank),  infos[1]),
                            (ECardColor)Enum.Parse(typeof(ECardColor), infos[2]));
    }

    #region Foundation

    private bool CanMoveFoundation(CardState card)
    {
        CardInfo info = GetCardInfo(card.cardName);

        return foundations[(int)info.cardSuit] == (int)info.cardRank - 1;
    }

    private void MoveToFoundation(CardState cardToMove, string movePoint)
    {
        CardInfo info = GetCardInfo(cardToMove.cardName);

        foundations[(int)info.cardSuit]++;

        moveCount++;

        logs.Add($"{movePoint}에 있던 Card_{info.cardRank}_{info.cardColor}를 Foundation {(int)info.cardSuit}번으로 이동");
    }

    #endregion

    #region Tableau

    private void MoveFromTableauToFoundation(int tableauIndex)
    {
        List<CardState> tableau = tableaus[tableauIndex];
        CardState tableauCard = tableau.Last();

        if (CanMoveFoundation(tableauCard) == false) return;

        tableau.Remove(tableauCard);

        if (tableau.Count != 0)
        {
            if (tableau.Last().isFaceUp == false)
            {
                CardState state = tableau[tableau.Count - 1];
                state.isFaceUp = true;
                tableau[tableau.Count - 1] = state;
            }
        }

        MoveToFoundation(tableauCard, $"Tablaeu {tableauIndex}");
    }

    private void MoveFromTableauToDiffTableau(int tableauIndex)
    {
        List<CardState> tableau = tableaus[tableauIndex];
        int i;

        for(i = 0; i < tableau.Count; i++)
        {
            if (tableau[i].movedDiffTableau == true ||
                tableau[i].isFaceUp == false) continue;

            FindMoveableDiffTableau(tableauIndex, tableau[i], out int moveableIndex);

            if(moveableIndex != -1)
            {
                CardState state = tableaus[tableauIndex][i];
                state.movedDiffTableau = true;
                tableaus[tableauIndex][i] = state;

                MoveToDiffTableau(tableauIndex, moveableIndex, i);

                break;
            }
        }
    }

    private bool CanMoveTableau(CardState from, CardState to)
    {
        CardInfo fromInfo = GetCardInfo(from.cardName);
        CardInfo toInfo   = GetCardInfo(to.cardName);

        if (fromInfo.cardRank + 1 == toInfo.cardRank &&
            fromInfo.cardColor    != toInfo.cardColor) return true;

        return false;
    }

    private void FindMoveableDiffTableau(int curTableauIndex, CardState cardState, out int moveableTableauIndex)
    {
        moveableTableauIndex = -1;

        for (int i = 0; i < tableaus.Length; i++)
        {
            if (i == curTableauIndex) continue;

            if (tableaus[i].Count == 0)
            {
                if (GetCardInfo(cardState.cardName).cardRank != ECardRank.K)
                    continue;
            }
            else
            {
                if (CanMoveTableau(cardState, tableaus[i].Last()) == false)
                    continue;
            }

            moveableTableauIndex = i;

            break;
        }
    }

    private void MoveToTableau(int moveTableau, CardState cardToMove, string movePoint)
    {
        tableaus[moveTableau].Add(cardToMove);

        moveCount++;

        CardInfo info = GetCardInfo(cardToMove.cardName);

        logs.Add($"{movePoint}에 있던 Card_{info.cardRank}_{info.cardColor}를 Tableau {moveTableau}로 이동");
    }

    private void MoveToDiffTableau(int fromTableau, int toTableau, int cardIndex)
    {
        List<CardState> childsIncludingSelf = new List<CardState>();
        int i;
        CardInfo info = GetCardInfo(tableaus[fromTableau][cardIndex].cardName);

        for (i = cardIndex; i < tableaus[fromTableau].Count; i++)
        {
            CardState cardState = tableaus[fromTableau][i];

            childsIncludingSelf.Add(cardState);

            if (cardState.isFaceUp == false)
            {
                CardState state = tableaus[fromTableau][i];
                state.isFaceUp = true;
                tableaus[fromTableau][i] = state;
            }
        }

        for (i = 0; i < childsIncludingSelf.Count; i++)
            tableaus[fromTableau].Remove(tableaus[fromTableau].Last());

        if (tableaus[fromTableau].Count != 0)
        {
            if (tableaus[fromTableau].Last().isFaceUp == false)
            {
                CardState state = tableaus[fromTableau].Last();
                state.isFaceUp = true;
                tableaus[fromTableau][tableaus[fromTableau].Count - 1] = state;
            }
        }

        tableaus[toTableau].AddRange(childsIncludingSelf);

        logs.Add($"Tableau {fromTableau}에 있던 Card_{info.cardRank}_{info.cardColor}를 Tableau {toTableau}로 이동");

        moveCount++;
    }

    #endregion

    #region Stock

    private void MoveFromStockToDiffPoints()
    {
        CardState stockCard;
        int i, j;

        List<CardState> stockCopy = new List<CardState>(stock);

        for (i = 0; i < stockCopy.Count; i++)
        {
            stockCard = stockCopy[i];

            if (CanMoveFoundation(stockCard))
            {
                stock.Remove(stockCard);

                MoveToFoundation(stockCard, "Stock");

                continue;
            }

            for (j = 0; j < tableaus.Length; j++)
            {
                if (tableaus[j].Count != 0)
                {
                    if (CanMoveTableau(stockCard, tableaus[j].Last()) == false)
                        continue;
                }
                else
                {
                    if (GetCardInfo(stockCard.cardName).cardRank != ECardRank.K)
                        continue;
                }

                stock.Remove(stockCard);

                stockCard.isFaceUp = true;

                MoveToTableau(j, stockCard, "Stock");
            }
        }
    }

    #endregion

    #region Find

    private struct PointType
    {
        public List<CardState> list;
        public int index;
    }

    private PointType FindCard(string cardInfoToFind)
    {
        int i;

        PointType cardPointToFind = new PointType();

        for (i = 0; i < tableaus.Length; i++)
        {
            for (int j = 0; j < tableaus[i].Count; j++)
            {
                if (tableaus[i][j].Equals(cardInfoToFind))
                {
                    cardPointToFind.list = tableaus[i];
                    cardPointToFind.index = j;

                    return cardPointToFind;
                }
            }
        }

        for (i = 0; i < stock.Count; i++)
        {
            if (stock[i].Equals(cardInfoToFind))
            {
                cardPointToFind.list = stock;
                cardPointToFind.index = i;

                return cardPointToFind;
            }
        }

        return cardPointToFind;
    }

    #endregion

    #region Swap

    private void Swap(string infoA, string infoB)
    {
        PointType aPoint = FindCard(infoA);
        PointType bPoint = FindCard(infoB);

        if (aPoint.list == null || bPoint.list == null) return;

        CardState temp = aPoint.list[aPoint.index];
        aPoint.list[aPoint.index] = bPoint.list[bPoint.index];
        bPoint.list[bPoint.index] = temp;
    }

    #endregion
}