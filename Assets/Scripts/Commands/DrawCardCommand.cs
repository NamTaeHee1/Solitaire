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
            while (Game.deckInWaste.Count > 0)
            {
                Card card = Game.deckInWaste.Last();

                card.Show(ECardDirection.BACK);
                card.Move(Managers.Point.stock);

                Game.deck.Add(card);
                Game.deckInWaste.Remove(card);
            }
        }
        else
        {
            Card card = Game.deck.Last();

            card.Show(ECardDirection.FRONT);
            card.Move(Managers.Point.waste);

            Game.deck.Remove(card);
            Game.deckInWaste.Add(card);
        }
    }

    public void Undo()
    {
        GameManager Game = Managers.Game;

        if (Game.deck.Count != 0)
        {
            Card card = Game.deckInWaste.Last();

            card.Show(ECardDirection.BACK);
            card.Move(Managers.Point.stock);

            Game.deckInWaste.Remove(card);
            Game.deck.Add(card);
        }
        else
        {

        }
    }
}
