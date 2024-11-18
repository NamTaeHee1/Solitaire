using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditorInternal;
using UnityEngine;

public class MoveCardCommand : ICommand
{
    private Card moveCard;
    private Point from, to;
    private ECardDirection fromDir;

    public MoveCardCommand(Card _moveCard, Point _from, Point _to)
    {
        moveCard = _moveCard;
        from = _from;
        to = _to;

        Card fromCard = null;

        if(from is Card) fromCard = (Card)from;

        int fromChild = from.transform.childCount;

        if (fromChild > 1)
            fromCard = from.transform.GetChild(fromChild - 2).GetComponent<Card>();

        if (fromCard == null) return;

        fromDir = fromCard.cardDirection;
    }

    public void Excute()
    {
        moveCard.Move(to);
    }

    public void Undo()
    {
        Card fromCard = from is Card ? (Card)from : from.GetLastCard();

        if(fromCard != null)
        {
            fromCard.Show(fromDir);
        }

        moveCard.Move(from);
    }
}
