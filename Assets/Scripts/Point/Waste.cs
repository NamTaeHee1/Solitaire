using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waste : Point
{
    #region Enter & Exit Point

    public override void OnEnterPoint(Card movedCard)
    {
        List<Card> wasteDeck = GameManager.Instance.deckInWaste;

        if (wasteDeck.Contains(movedCard) == false)
        {
            wasteDeck.Add(movedCard);
        }
    }

    public override void OnExitPoint(Card movedCard)
    {
        List<Card> wasteDeck = GameManager.Instance.deckInWaste;

        if(wasteDeck.Contains(movedCard))
        {
            wasteDeck.Remove(movedCard);
        }
    }

    #endregion

}