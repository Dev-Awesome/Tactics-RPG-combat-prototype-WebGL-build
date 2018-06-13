using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour {

	private static Singleton _instance;
	public static Singleton instance { get { return _instance; } }

	void Awake()
	{
		if (_instance != null && _instance != this)
		{
			Destroy(this.gameObject);
		} else {
			_instance = this;
		}
	}

	void OnDestroy()
	{
		if (this == _instance) { 
			_instance = null;
		}
	}
}
