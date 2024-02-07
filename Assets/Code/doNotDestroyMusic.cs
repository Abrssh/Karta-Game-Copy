using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class doNotDestroyMusic : MonoBehaviour {

	void Awake()
	{
		if(gameObject.tag=="charles")
		{
			GameObject[] charles = GameObject.FindGameObjectsWithTag("charles");
			if (charles.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
		else if(gameObject.tag=="bethoven")
		{
			GameObject[] bethoven = GameObject.FindGameObjectsWithTag("bethoven");
			if (bethoven.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
		else if (gameObject.tag == "mozart1")
		{
			GameObject[] mozart1 = GameObject.FindGameObjectsWithTag("mozart1");
			if (mozart1.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
		else if (gameObject.tag == "mozart2")
		{
			GameObject[] mozart2 = GameObject.FindGameObjectsWithTag("mozart2");
			if (mozart2.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
		else if (gameObject.tag == "mozart3")
		{
			GameObject[] mozart3 = GameObject.FindGameObjectsWithTag("mozart3");
			if (mozart3.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
		else if (gameObject.tag == "bach")
		{
			GameObject[] bach = GameObject.FindGameObjectsWithTag("bach");
			if (bach.Length > 1)
				Destroy(this.gameObject);

			DontDestroyOnLoad(this.gameObject);
		}
	}
}
