using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool IsWin()
    {
        Point[] foundations = Managers.Point.foundations;
        Card lastCard;

        for(int i = 0; i < foundations.Length; i++)
        {
            lastCard = foundations[i].GetLastCard();

            if (lastCard == null) return false;

            if (lastCard.cardInfo.cardRank != ECardRank.K) return false;
        }

        return true;
    }
}