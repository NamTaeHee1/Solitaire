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
}