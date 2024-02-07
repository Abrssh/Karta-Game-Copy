using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doNot : MonoBehaviour
{
	void Awake()
	{
		GameObject[] charles = GameObject.FindGameObjectsWithTag("charles");
		if (gameObject.tag == "charles")
		{
			if (charles.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
	}
}
