using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
	public static RaycastHit2D RaycastMousePos(Camera cam, LayerMask layer)
	{
		RaycastHit2D hit = Physics2D.Raycast(cam.ScreenToWorldPoint(Input.mousePosition),
											 Vector2.zero,
											 0f,
											 layer);

		return hit;
	}

    public static Card GetCard(string cardName)
    {
        var cardNameDict = Managers.Point.stock.cardNameDict;

        cardNameDict.TryGetValue(cardName, out Card card);

        return card;
    }

    public static CardInfo ToCardInfo(this string cardName)
    {
        CardInfo cardInfo = new CardInfo();

        string[] cardInfoArr = cardName.Split('_'); // [1] : Suit, [2] : Rank, [3] : Color
        cardInfo.cardSuit = (ECARD_SUIT)Enum.Parse(typeof(ECARD_SUIT), cardInfoArr[1].ToUpper());
        cardInfo.cardRank = (ECARD_RANK)Enum.Parse(typeof(ECARD_RANK), cardInfoArr[2].ToUpper());
        cardInfo.cardColor = (ECARD_COLOR)Enum.Parse(typeof(ECARD_COLOR), cardInfoArr[3].ToUpper());

        return cardInfo;
    }
}