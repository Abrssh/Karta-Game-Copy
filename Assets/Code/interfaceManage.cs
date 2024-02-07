using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class interfaceManage : MonoBehaviour {

	[SerializeField] private GameObject makeAbid;
	[SerializeField] private GameObject scoreBoard;
	[SerializeField] private GameObject quitMenu;

	[SerializeField] Management manage;

	public TextMeshProUGUI bid;

	public TextMeshProUGUI roundUI;

	bool oneTimeOnForBid=true;

	[SerializeField] private TextMeshProUGUI[] scoreBoardTxt;

	[SerializeField] private TextMeshProUGUI[] playerBidsTxts;
	[SerializeField] private Slider slide;


	// Use this for initialization
	void Start () {


	}

	public void loadScene()// reset the static script to zero for a new game and load the current Scene
	{
		for (int i = 0; i < 4; i++)
		{
			staticScript.round1[i] = 0;
			staticScript.round2[i] = 0;
			staticScript.round3[i] = 0;
			staticScript.round4[i] = 0;
			staticScript.round5[i] = 0;
		}
		staticScript.roundCounter = 1;
		SceneManager.LoadScene(1);
	}
	
	// Update is called once per frame
	void Update () {
		
		if(manage.animationControl==true && oneTimeOnForBid==true)
		{
			makeAbid.SetActive(true);
			oneTimeOnForBid=false;
		}
	}

	public void load_mainMenu() // reset the static script to zero for a new game and load mainMenu
	{
		for (int i = 0; i < 4; i++)
		{
			staticScript.round1[i] = 0;
			staticScript.round2[i] = 0;
			staticScript.round3[i] = 0;
			staticScript.round4[i] = 0;
			staticScript.round5[i] = 0;
		}
		staticScript.roundCounter = 1;
		SceneManager.LoadScene(0);
	}

	public void hideQuitMenu()
	{
		quitMenu.SetActive(false);
	}

	public void showQuitMenu()
	{
		quitMenu.SetActive(true);
	}

	public void unshowMakeAbid()
	{
		makeAbid.SetActive(false);
		staticScript.playerBid=int.Parse(bid.text);// after the right(nike) button is clicked
		manage.playerBidComplete=true;
	}

	public void showScoreBoard()
	{
		scoreBoard.SetActive(true);
	}

	public void HideScoreBoard()
	{
		scoreBoard.SetActive(false);
		if(staticScript.roundCounter==6)// starts a new Game
		{
			for (int i = 0; i < 4; i++)
			{
				staticScript.round1[i] = 0;
				staticScript.round2[i] = 0;
				staticScript.round3[i] = 0;
				staticScript.round4[i] = 0;
				staticScript.round5[i] = 0;
			}
			staticScript.roundCounter = 1;
			StartCoroutine(manage.deleteAndLoad());
		}
	}

	public void addBid()
	{
		int a= int.Parse(bid.text);
		if(a<=12)
		{
			a++;
			bid.text=a.ToString();
			if(slide.value<slide.maxValue)
			{
				slide.value++;
			}
		}
	}
	
	public void subBid()
	{
		int a= int.Parse(bid.text);
		if(a>=2)
		{
			a--;
			bid.text=a.ToString();
			if(slide.value>slide.minValue && a<8)
			{
				slide.value--;
			}
		}
	}

	public void AssignPlayersBid(int[] bid)
	{
		playerBidsTxts[0].text=bid[0].ToString();
		playerBidsTxts[1].text=bid[1].ToString();
		playerBidsTxts[2].text=bid[2].ToString();
		playerBidsTxts[3].text=bid[3].ToString();
	}
	public void AssignPlayersValue(int[] value)
	{
		playerBidsTxts[4].text=value[0].ToString();
		playerBidsTxts[5].text=value[1].ToString();
		playerBidsTxts[6].text=value[2].ToString();
		playerBidsTxts[7].text=value[3].ToString();
	}

	public float[] returnBid()
	{
		float[] bid = new float[4];
		for (int i = 0; i < 4; i++)
		{
			bid[i] = float.Parse(playerBidsTxts[i].text);
		}
		return bid;
	}

	public float[] returnValue()
	{
		float[] value = new float[4];
		int j = 0;
		for (int i = 4; i < 8; i++)
		{
			value[j] = float.Parse(playerBidsTxts[i].text);
			j++;
		}
		return value;
	}

	public void AssignScoreBoard() // assign the score of a round stored in the static script when 
	// the scene reload and when to be assigned is determined by the roundCounter found in static script
	// which basically determines in this case the number of round played
	{
		int j=0;
		for (int i = 0; i < 20; i++)
		{
			if(i<4)
			{
				if(staticScript.roundCounter>=1)
				{
					scoreBoardTxt[i].enabled = true;
					scoreBoardTxt[i].text = staticScript.round1[j].ToString();
				}
			}
			else if(i>=4 && i<8)
			{
				if(staticScript.roundCounter>1)
				{
					scoreBoardTxt[i].enabled = true;
					scoreBoardTxt[i].text = staticScript.round2[j].ToString();
				}
			}
			else if(i>=8 && i<12)
			{
				if(staticScript.roundCounter>2)
				{
					scoreBoardTxt[i].enabled = true;
					scoreBoardTxt[i].text = staticScript.round3[j].ToString();
				}
			}
			else if(i>=12 && i<16)
			{
				if(staticScript.roundCounter>3)
				{
					scoreBoardTxt[i].enabled = true;
					scoreBoardTxt[i].text = staticScript.round4[j].ToString();
				}
			}
			else
			{
				if(staticScript.roundCounter>4)
				{
					scoreBoardTxt[i].enabled = true;
					scoreBoardTxt[i].text = staticScript.round5[j].ToString();
				}
			}
			if(j<3)  
			{
				j++;
			}
			else
			{
				j=0;
			}
		}
	}

	public void WinnerChickenDinner(string[] winner,float[] FINResult)
	{
		int j=0,q=0;
		for (int i = 20; i < 24; i++)
		{
			scoreBoardTxt[i].enabled = true;
			scoreBoardTxt[i].text = FINResult[q].ToString();
			q++;
		}
		for (int i = 24; i < 28; i++)
		{
			scoreBoardTxt[i].enabled=true;
			scoreBoardTxt[i].text=winner[j];
			j++;
		}
	}
	
	public void onSliderChange()// assign the new slider value to the bid text
	{
		bid.text = slide.value.ToString();
	}
}
