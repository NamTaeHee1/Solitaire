using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using static SolitaireSolver;
using Random = System.Random;

public class SolitaireSolver
{
    public struct CardState
    {
        public CardInfo cardInfo;
        public bool isFaceUp;
        public bool movedDiffTableau;

        public CardState(CardInfo _cardInfo, bool _isFaceUp)
        {
            cardInfo = _cardInfo;
            isFaceUp = _isFaceUp;
            movedDiffTableau = false;
        }

        public override bool Equals(object obj)
        {
            return ((CardState)obj).cardInfo.Equals(this.cardInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public struct Deck
    {
        public List<CardState>[] tableaus;
        public List<CardState> stock;
        public int[] foundations;

        public Deck(List<CardState>[] _tableaus, List<CardState> _stock, int[] _foundation)
        {
            tableaus = _tableaus;
            stock = _stock;
            foundations = _foundation;
        }

        // 깊은 복사
        public Deck Clone()
        {
            Deck newDeck = new Deck();

            newDeck.tableaus = new List<CardState>[tableaus.Length];

            for (int i = 0; i < tableaus.Length; i++)
                newDeck.tableaus[i] = new List<CardState>(tableaus[i]);

            newDeck.stock = new List<CardState>(stock);

            newDeck.foundations = new int[foundations.Length];

            for(int i = 0; i < foundations.Length; i++)
                newDeck.foundations[i] = foundations[i];

            return newDeck;
        }

        public bool IsSolvable()
        {
            int faceUpCount = 0, i;

            for(i = 0; i < tableaus.Length; i++)
            {
                for(int j = 0; j < tableaus[i].Count; j++)
                {
                    if (tableaus[i][j].isFaceUp == true) faceUpCount++;
                }
            }

            faceUpCount += stock.Count;

            for (i = 0; i < foundations.Length; i++)
                faceUpCount += foundations[i];

            return faceUpCount >= DEFINE.CARD_MAX_SIZE;
        }
    }

    private Deck Init(List<string> deckToString)
    {
        List<CardState>[] tableaus = new List<CardState>[7];
        List<CardState> stock = new List<CardState>();

        int i, count = 0;
        CardState lastCard;

        for (i = 0; i < tableaus.Length; i++)
            tableaus[i] = new List<CardState>();

        for (i = 0; i < tableaus.Length; i++)
        {
            for (int j = i; j < tableaus.Length; j++)
            {
                lastCard = new CardState(GetCardInfo(deckToString[count++]), i == j);

                tableaus[j].Add(lastCard);
            }
        }

        for (i = count; i < deckToString.Count; i++)
        {
            lastCard = new CardState(GetCardInfo(deckToString[count++]), true);

            stock.Add(lastCard);
        }

        return new Deck(tableaus, stock, new int[4] {0, 0, 0, 0});
    }

    private Random rand = new Random();

    public bool IsSolve(List<string> deckToString)
    {
        Deck deck = Init(deckToString);

        return Solve(deck);
    }

    public bool Solve(Deck deck)
    {
        bool changedDeck = false;

        for (int i = 0; i < deck.tableaus.Length; i++)
        {
            MoveFromTableauToFoundation(i, deck, out changedDeck);

            MoveFromTableauToDiffTableau(i, deck, out changedDeck);
        }

        MoveFromStockToDiffPoints(deck, out changedDeck);

        if (deck.IsSolvable())
            return true;

        if (changedDeck)
            return Solve(deck);
        else
            return false;
    }

    #region CardInfo

    private CardInfo GetCardInfo(string cardInfoStr)
    {
        string[] infos = cardInfoStr.Split('_');

        return new CardInfo((ECardSuit) Enum.Parse(typeof(ECardSuit),  infos[0]),
                            (ECardRank) Enum.Parse(typeof(ECardRank),  infos[1]),
                            (ECardColor)Enum.Parse(typeof(ECardColor), infos[2]));
    }

    #endregion

    #region Foundation

    private bool CanMoveFoundation(Deck deck, CardState card)
    {
        CardInfo info = card.cardInfo;

        // foundations 배열은 0, 0, 0, 0 인데 cardRank의 첫 요소인 A는 1로 시작해서 -1 해줌.
        return deck.foundations[(int)info.cardSuit] == (int)info.cardRank - 1;
    }

    private void MoveToFoundation(Deck deck, CardState cardToMove)
    {
        CardInfo info = cardToMove.cardInfo;

        deck.foundations[(int)info.cardSuit]++;
    }

    #endregion

    #region Tableau

    private void MoveFromTableauToFoundation(int tableauIndex, Deck deck, out bool changedDeck)
    {
        changedDeck = false;

        List<CardState> tableau = deck.tableaus[tableauIndex];

        if (tableau.Count == 0) return;

        CardState tableauCard = tableau.Last();

        if (CanMoveFoundation(deck, tableauCard) == false) return;

        tableau.Remove(tableauCard);

        MoveToFoundation(deck, tableauCard);

        changedDeck = true;

        if (tableau.Count == 0) return;

        if (tableau.Last().isFaceUp == true) return;

        CardState state = tableau[tableau.Count - 1];
        state.isFaceUp = true;
        tableau[tableau.Count - 1] = state;
    }

    private void MoveFromTableauToDiffTableau(int tableauIndex, Deck deck, out bool changedDeck)
    {
        changedDeck = false;

        List<CardState> tableau = deck.tableaus[tableauIndex];
        
        for(int i = 0; i < tableau.Count; i++)
        {
            if (tableau[i].movedDiffTableau == true ||
                tableau[i].isFaceUp == false) continue;

            FindMoveableDiffTableau(tableauIndex, tableau[i], deck, out int moveableIndex);

            if(moveableIndex != -1)
            {
                tableau = deck.tableaus[tableauIndex];

                CardState state = tableau[i];
                state.movedDiffTableau = true;
                tableau[i] = state;

                MoveToDiffTableau(tableauIndex, moveableIndex, i, deck.tableaus);

                changedDeck = true;

                break;
            }
        }
    }

    private bool CanMoveTableau(CardState from, CardState to)
    {
        CardInfo fromInfo = from.cardInfo;
        CardInfo toInfo   = to.cardInfo;

        if (fromInfo.cardRank + 1 == toInfo.cardRank &&
            fromInfo.cardColor    != toInfo.cardColor) return true;

        return false;
    }

    private void FindMoveableDiffTableau(int curTableauIndex, CardState cardState, Deck deck, out int moveableTableauIndex)
    {
        moveableTableauIndex = -1;

        for (int i = 0; i < deck.tableaus.Length; i++)
        {
            if (i == curTableauIndex) continue;

            if (deck.tableaus[i].Count == 0)
            {
                if (cardState.cardInfo.cardRank != ECardRank.K)
                    continue;
            }
            else
            {
                if (CanMoveTableau(cardState, deck.tableaus[i].Last()) == false)
                    continue;
            }

            moveableTableauIndex = i;

            break;
        }
    }

    private void MoveToTableau(int moveTableau, CardState cardToMove, Deck deck)
    {
        deck.tableaus[moveTableau].Add(cardToMove);
    }

    private void MoveToDiffTableau(int fromTableau, int toTableau, int cardIndex, List<CardState>[] tableaus)
    {
        List<CardState> childsIncludingSelf = new List<CardState>();
        int i;
        CardInfo info = tableaus[fromTableau][cardIndex].cardInfo;

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
    }

    #endregion

    #region Stock

    private void MoveFromStockToDiffPoints(Deck deck, out bool changedDeck)
    {
        changedDeck = false;

        CardState stockCard;

        List<CardState> stockCopy = new List<CardState>(deck.stock);

        for (int i = 0; i < stockCopy.Count; i++)
        {
            stockCard = stockCopy[i];

            if (CanMoveFoundation(deck, stockCard))
            {
                deck.stock.Remove(stockCard);

                MoveToFoundation(deck, stockCard);

                changedDeck = true;

                continue;
            }

            for (int j = 0; j < deck.tableaus.Length; j++)
            {
                if (deck.tableaus[j].Count != 0)
                {
                    if (CanMoveTableau(stockCard, deck.tableaus[j].Last()) == false)
                        continue;
                }
                else
                {
                    if (stockCard.cardInfo.cardRank != ECardRank.K)
                        continue;
                }

                deck.stock.Remove(stockCard);

                MoveToTableau(j, stockCard, deck);

                changedDeck = true;
            }
        }
    }

    #endregion
}