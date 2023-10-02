using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
	public static T GetOrAddComponent<T>(this GameObject _gameObject) where T : Component
	{
		Component _component = _gameObject.GetComponent<T>();
		if(_component  ==  null)
			_component = _gameObject.AddComponent<T>();

		return (T)_component;
	}
}
