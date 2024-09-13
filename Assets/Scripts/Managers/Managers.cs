using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Managers : MonoBehaviour
{
	public static InputManager Input { get { return InputManager.Instance; } }
	
	public static PointManager Point { get { return PointManager.Instance; } }

	public static GameManager Game { get { return GameManager.Instance; } }
}
