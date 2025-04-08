using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

public class HintSystem
{
    #region Move

    public void ExploreAndMove()
    {
        for(int i = 0; i < Managers.Point.tableaus.Length; i++)
        {
            if (MoveToTableau(i)) return;
        }
        
        if (MoveToFoundation()) return;
    }

    private void MoveCard(Card moveCard, Point toPoint)
    {
        MoveCardCommand command = new MoveCardCommand(moveCard, moveCard.curPoint, toPoint);

        command.Excute();

        AddLogs(moveCard.cardInfo, toPoint);

        Recorder.Push(command);
    }

    #endregion

    #region Tableau

    private bool MoveToTableau(int index)
    {
        Tableau[] tableaus = Managers.Point.tableaus;
        GameManager Game = Managers.Game;

        for(int i = 0; i < tableaus[index].transform.childCount; i++)
        {
            Card childCard = tableaus[index].transform.GetChild(i).GetComponent<Card>();

            if (childCard.cardDirection == ECardDirection.BACK) continue;

            for (int j = 0; j < tableaus.Length; j++)
            {
                if (j == index) continue;

                if (CanMoveTableau(tableaus[j], childCard, out Point pointToMove))
                {
                    MoveCard(childCard, pointToMove);

                    return true;
                }
            }
        }

        if (Game.deckInWaste.Count == 0)
        {
            Managers.Point.stock.DrawCard();

            return true;
        }

        Card wasteLastCard = Game.deckInWaste.Last();
        Card lastCard = tableaus[index].GetLastCard();

        if (lastCard == null)
        {
            if (wasteLastCard.cardInfo.cardRank == ECardRank.K)
            {
                MoveCard(wasteLastCard, tableaus[index]);

                return true;
            }

            return false;
        }

        CardInfo lastCardInfo = lastCard.cardInfo;
        CardInfo wasteCardInfo = wasteLastCard.cardInfo;

        if (lastCardInfo.cardRank == wasteCardInfo.cardRank + 1 && IsSameColor(lastCardInfo, wasteCardInfo) == false)
        {
            MoveCard(wasteLastCard, tableaus[index].GetLastCard());

            return true;
        }

        return false;
    }

    private bool CanMoveTableau(Tableau tableau, Card moveCard, out Point pointToMove)
    {
        Card lastCard = tableau.GetLastCard();

        CardInfo moveCardInfo = moveCard.cardInfo;

        pointToMove = null;

        if (lastCard == null)
        {
            if (moveCardInfo.cardRank == ECardRank.K && IsMoved(moveCardInfo, tableau) == false)
            {
                pointToMove = tableau;

                return true;
            }

            return false;
        }

        if (IsMoved(moveCardInfo, lastCard)) return false;

        bool isSuitable = (moveCardInfo.cardRank + 1 == lastCard.cardInfo.cardRank && IsSameColor(moveCardInfo, lastCard.cardInfo) == false);

        if (isSuitable) pointToMove = lastCard;

        return isSuitable;
    }

    #endregion

    #region Foundation

    private bool MoveToFoundation()
    {
        Tableau[] tableaus = Managers.Point.tableaus;
        Foundation[] foundations = Managers.Point.foundations;
        GameManager Game = Managers.Game;

        for(int i = 0; i < tableaus.Length; i++)
        {
            Card lastCard = tableaus[i].GetLastCard();

            if (lastCard == null) continue;

            if(CanMoveFoundation(lastCard.cardInfo))
            {
                MoveCard(lastCard, foundations[(int)lastCard.cardInfo.cardSuit]);

                return true;
            }
        }

        if (Game.deckInWaste.Count == 0) return false;

        Card wasteLastCard = Game.deckInWaste.Last();

        if (CanMoveFoundation(wasteLastCard.cardInfo))
        {
            MoveCard(wasteLastCard, foundations[(int)wasteLastCard.cardInfo.cardSuit]);

            return true;
        }

        Managers.Point.stock.DrawCard();

        return false;
    }

    private bool CanMoveFoundation(CardInfo cardInfo)
    {
        Foundation[] foundations = Managers.Point.foundations;

        if (cardInfo.cardRank == ECardRank.A) return true;

        Card foundationLastCard = foundations[(int)cardInfo.cardSuit].GetLastCard();

        if (foundationLastCard == null) return false;

        return foundationLastCard.cardInfo.cardRank + 1 == cardInfo.cardRank;
    }

    #endregion

    #region Move Logs

    private List<string> moveLogs = new List<string>();

    private void AddLogs(CardInfo cardInfo, Point toPoint)
    {
        moveLogs.Add($"{cardInfo.ToString()}_{toPoint.name}");
    }

    private bool IsMoved(CardInfo cardInfo, Point toPoint)
    {
        return moveLogs.Contains($"{cardInfo.ToString()}_{toPoint.name}");
    }
    
    public void ClearLogs()
    {
        moveLogs.Clear();
    }

    #endregion

    #region Util

    private bool IsSameColor(CardInfo cardA,  CardInfo cardB)
    {
        return cardA.cardColor == cardB.cardColor;
    }

    #endregion
}
