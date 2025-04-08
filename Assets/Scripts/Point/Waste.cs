using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waste : Point
{
    #region Enter & Exit Point

    public override void OnEnterPoint(Card movedCard)
    {
        List<Card> wasteDeck = Managers.Game.deckInWaste;

        if (wasteDeck.Contains(movedCard) == false)
        {
            wasteDeck.Add(movedCard);

            SortCardZ();
        }
    }

    public override void OnExitPoint(Card movedCard)
    {
        List<Card> wasteDeck = Managers.Game.deckInWaste;

        if(wasteDeck.Contains(movedCard))
        {
            wasteDeck.Remove(movedCard);
        }
    }

    private void SortCardZ()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            Vector3 cardPos = transform.GetChild(i).localPosition;

            cardPos.z = -(i + 1) * 0.1f;

            transform.GetChild(i).localPosition = cardPos;
        }
    }

    #endregion

}