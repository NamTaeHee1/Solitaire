using System.Collections;
using System.Collections.Generic;
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
}
