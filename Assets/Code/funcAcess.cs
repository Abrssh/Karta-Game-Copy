using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.Internal.Experimental.UIElements;
using UnityEngine.Monetization;
using UnityEngine.SceneManagement;

public class funcAcess : MonoBehaviour {

	[SerializeField] GameObject quitMenu;
	[SerializeField] GameObject panel;
	[SerializeField] GameObject warningSign;
	bool startShowingWarningSign = false;

	private float interpolationTime = 1.5f;
	private float time = 0f;
	
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {

		if(startShowingWarningSign==true)
		{
			time += Time.deltaTime;
			if(time>=interpolationTime)
			{
				time -= interpolationTime;
				startShowingWarningSign = false;
				warningSign.SetActive(false);
			}
		}
	}

	public void PlayGame()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
	}

	public void HideQuitMenu()
	{
		quitMenu.SetActive(false);
	}
	public void ShowQuitMenu()
	{
		quitMenu.SetActive(true);
	}

	public void QuitGame()
	{
		// shut Down the Game
		Application.Quit();
	}
	public void ShowHelp()
	{
		panel.SetActive(true);
	}
	public void HideHelp()
	{
		panel.SetActive(false);
	}

	public void showWarningSign()
	{
		if(startShowingWarningSign==true)
		{
			// means warning sign is active
		}
		else
		{
			warningSign.SetActive(true);
			startShowingWarningSign = true;
		}
	}
}
