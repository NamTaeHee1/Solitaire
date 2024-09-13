using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Stock : Point
{
	[Header("클릭 시 이동 Point")][SerializeField]
	private Point wastePoint;

	private Camera mainCam;

	private LayerMask stockLayer;

	private void Awake()
	{
		mainCam = Camera.main;

		stockLayer = 1 << LayerMask.NameToLayer("Stock");
	}

	private void Update()
	{
		StockPointClick();
	}

	private void StockPointClick()
	{
		if (Managers.Input.canInput == false) return;

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit2D hit = Utils.RaycastMousePos(mainCam, stockLayer);

			if (hit == false) return;

			if (Managers.Game.deck.Count == 0)
				BackToStock();
			else
				PickCardFromDeck();
		}
	}

	private void PickCardFromDeck()
	{
		GameManager Game = Managers.Game;

		Card pickCard = Game.deck.First<Card>();

		pickCard.Show(ECardDirection.FRONT);
		pickCard.Move(wastePoint);

		Game.deck.Remove(pickCard);
		Game.deckInWaste.Add(pickCard);
	}

	private void BackToStock()
	{
		GameManager Game = Managers.Game;

		while(Game.deckInWaste.Count > 0)
		{
			Card card = Game.deckInWaste.First<Card>();

			card.Show(ECardDirection.BACK);
			card.Move(this);

			Game.deck.Add(card);
			Game.deckInWaste.Remove(card);
		}
	}
}
