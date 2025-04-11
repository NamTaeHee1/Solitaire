using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointManager : SingletonMono<PointManager>
{
	public Tableau[] tableaus;
	public Foundation[] foundations;
	public Stock stock;
    public Waste waste;

    private EHAND_DIRECTION curHandDirection;

    public void SetHandDirection(EHAND_DIRECTION direction)
    {
        if (curHandDirection == direction)
            return;

        curHandDirection = direction;

        int stockMultiply = direction == EHAND_DIRECTION.LEFT ? -1 : 1;
        int foundationMultiply = direction == EHAND_DIRECTION.LEFT ? 1 : -1;

        for(int i = 0; i < foundations.Length; i++)
        {
            Vector3 foundationPos = foundations[i].transform.position;

            foundationPos.x = Mathf.Abs(foundationPos.x);
            foundationPos.x *= foundationMultiply;

            foundations[i].transform.position = foundationPos;
        }

        Vector3 stockPos = stock.transform.position;

        stockPos.x = Mathf.Abs(stockPos.x);
        stockPos.x *= stockMultiply;

        stock.transform.position = stockPos;

        Vector3 wastePos = waste.transform.position;

        wastePos.x = Mathf.Abs(wastePos.x);
        wastePos.x *= stockMultiply;
        wastePos.z = -3f;

        waste.transform.position = wastePos;
    }
}
