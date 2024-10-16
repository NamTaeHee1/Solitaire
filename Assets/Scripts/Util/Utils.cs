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

    public static int Max(this int[] array)
    {
        int maxNum = array[0];

        for(int i = 1; i < array.Length; i++)
        {
            if(array[i] > maxNum)
                maxNum = array[i];
        }

        return maxNum;
    }

    public static int Min(this int[] array)
    {
        int minNum = array[0];

        for(int i = 1; i < array.Length; i++)
        {
            if(array[i] < minNum)
                minNum = array[i];
        }

        return minNum;
    }
}