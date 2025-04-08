using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DrawCardCommand : ICommand
{
    public void Excute()
    {
        GameManager Game = Managers.Game;

        if (Game.deck.Count == 0)
        {
            Managers.Sound.Play("MoveCard");

            while (Game.deckInWaste.Count > 0)
            {
                Card card = Game.deckInWaste.Last();

                card.Show(ECARD_DIRECTION.BACK);
                card.Move(Managers.Point.stock, 0f, false);

                Game.deck.Add(card);
            }

            Debug.Log($"Deck Add");
        }
        else
        {
            Card card = Game.deck.Last();

            card.Show(ECARD_DIRECTION.FRONT);
            card.Move(Managers.Point.waste);

            Game.deck.Remove(card);

            Debug.Log($"Deck Remove");
        }
    }

    public void Undo()
    {
        GameManager Game = Managers.Game;

        if (Game.deckInWaste.Count == 0)
        {
            while(Game.deck.Count > 0)
            {
                Card card = Game.deck.Last();

                card.Show(ECARD_DIRECTION.FRONT);
                card.Move(Managers.Point.waste);

                Game.deck.Remove(card);
            }
        }
        else
        {
            Card card = Game.deckInWaste.Last();

            card.Show(ECARD_DIRECTION.BACK);
            card.Move(Managers.Point.stock);

            Game.deck.Add(card);
        }
    }
}
