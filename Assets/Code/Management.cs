using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.Monetization;
using UnityEngine.Advertisements;

public class Management : MonoBehaviour {

    public Sprite[] cardImages;
    [SerializeField] private GameObject[] cards;

    [SerializeField] private GameObject[] cardDropPlace; // replace with the positions of the gameObjects

    [SerializeField] private GameObject[] opponentCardBack;

    [SerializeField] private GameObject[] otherPlayerCard; // replace with the positions of the gameObjects

    [SerializeField] private interfaceManage interfaceManage;

    int[] arrayforCards = new int[52];
    public int[] arrayCard1;

    int[] arrayCard2, arrayCard3, arrayCard4;

    Vector3 tempScale=new Vector3(0.2466041f,0.2616924f,0.2466041f);
    Vector3 originalScale=new Vector3(0.2466041f,0.2616924f,0.2466041f);

    Vector3 DestinationPlace;

    List<GameObject> clonesCards=new List<GameObject>();

    float time=0,interpolationTime=0.5f;
    // Use this for initialization

    control_anim cardBackAnim;

    Vector3[] startPosForOtherPlayerCard=new Vector3[4];
    Vector3[] cardDropPositons=new Vector3[4];

	public bool touchControl=false,OneCardController=false;// touch control used to make sure cards cant be 
    //touched before the player turn and one cardController used to make sure only one card at a time can be thrown
	public int ActiveCard=0;
    int activeCardForGameType = 0;

	public int gameControl=0,Counter=0;

	public float speed=10.0f;

    public bool[] test={false,false,false,false};
    int[] notAvailable; // used to hold index of cards to be lowered

    public const float defaultYCardPositon = -3.835f;

    SpriteRenderer sr0,sr1,sr2,srPlayer;
    
    bool disappear=false,waitAndRestart=true,usedForMoveTowards=false,throwAfter=true;
    // usedForMoveTowards is used to control when justMove function is called in update since
    // the funtion moveTowards is dependent on update function
    // throwAfter used to control opponennt to throw their cards after all the previous cards
    // have been destroyed

    public bool animationControl=false; // used to tell other players to wait until the initial animation ends

    int winnerOfOneHit=0;

    GameObject card1,card2,card3,card4;
    Vector3 tempPosition=Vector3.one;


    Vector3 eulerPlayerAngles;

    public int[] bid =new int[4],value=new int[4];// used to hold the bid and score value of
    //a player bid and value for one round

    public bool playerBidComplete=false,waitTothrowUntilBidComplete=false;
    int throwCount=0; // used to hold how much throw has been performed and notify the end has been reached
    // so terminate delete the opponenet card collection

    public GameObject gameType; // used to show the current game type of this hit
    public Sprite[] gameTypeSprite; // holds the game type sprites

    Image gameTypeImage;

    //Variables for Audio
    private AudioSource[] backgroundAudio;
    // used to store audio in a prefab to be played
    [SerializeField] GameObject[] ObjAudio;
    private GameObject[] instantiateAudio;
    //int currentAudioNumber = 0; // used to identify the number of track currently playing or about to play

    // Play and Pause for audio
    [SerializeField] GameObject PlayBtn, PauseBtn;

    //Variable for AD
    private string adID = "3682603";


    // Left of at : Synchronize the animation with the movement and make sure 
    // moving cards have been destroyed before throwing another card using boolean variables :: Solved
    // create the score system for one round and UI system plus 
    //make the movement to the winner player :: Solved 
    //Synchronize the animation and general polish awaits which includes removing
    // comments and unused function and variables


    // error: you can throw more than one cards at a time if you are fast enough which 
    // it should not be allowed :: Solved
    // error: it sometimes raise the wrong cards so check the lower and climb cards

    /* Polish: Order the middle cards when they get instantiated with order layer so they won't be on
     on top each other creating weird motions
     2 Use wait and bool to synchronize the animation with opponent card throwing and let them wait between
     each other
     */

    void Start()
    {
        arrayforCards = fillNumber();
        arrayforCards = ShuffleArray(arrayforCards);


        arrayCard1 = fillArray(arrayforCards, 0); // realPlayer array
        arrayCard2 = fillArray(arrayforCards, 13);// bot-1
        arrayCard3 = fillArray(arrayforCards, 26);// bot-2
        arrayCard4 = fillArray(arrayforCards, 39);// bot-3

        arrayCard1=bubbleSort(arrayCard1);
        arrayCard2=bubbleSort(arrayCard2);
        arrayCard3=bubbleSort(arrayCard3);
        arrayCard4=bubbleSort(arrayCard4);

        SpriteRenderer sr = new SpriteRenderer();

        for (int i = 0; i < cards.Length; i++)
        {
            sr = cards[i].GetComponent<SpriteRenderer>();
            sr.sprite = cardImages[arrayCard1[i]];
        }

        StartCoroutine(PlayerCardAnimation());

        DestinationPlace=new Vector3(cardDropPlace[3].transform.position.x
        ,cardDropPlace[3].transform.position.y,cardDropPlace[3].transform.position.z);

        startPosForOtherPlayerCard[0]=otherPlayerCard[0].transform.position;
        startPosForOtherPlayerCard[1]=otherPlayerCard[1].transform.position;
        startPosForOtherPlayerCard[2]=otherPlayerCard[2].transform.position;
        startPosForOtherPlayerCard[3]=otherPlayerCard[3].transform.position;

        cardDropPositons[0]=cardDropPlace[0].transform.position;
        cardDropPositons[1]=cardDropPlace[1].transform.position;
        cardDropPositons[2]=cardDropPlace[2].transform.position;
        cardDropPositons[3]=cardDropPlace[3].transform.position;

        eulerPlayerAngles=cardDropPlace[3].transform.eulerAngles;

		gameControl=Random.Range(0,4);

        //Debug.Log("Game control >> "+gameControl);

        if(gameControl==0)
        {
            test[0]=true;
        }
        else if(gameControl==1)
        {
            test[1]=true;
        }
        else if(gameControl==2)
        {
            test[2]=true;
        }
        else
        {
            touchControl=true;// unlock the touch ability
            OneCardController=true;
        }

        sr0=new SpriteRenderer();
        sr0=cardDropPlace[0].GetComponent<SpriteRenderer>();
        sr1=new SpriteRenderer();
        sr1=cardDropPlace[1].GetComponent<SpriteRenderer>();
        sr2=new SpriteRenderer();
        sr2=cardDropPlace[2].GetComponent<SpriteRenderer>();
        srPlayer=new SpriteRenderer();

        bid[0] = bidGuess(arrayCard2);
        bid[1] = bidGuess(arrayCard3);
        bid[2] = bidGuess(arrayCard3);

        for (int i = 0; i < value.Length; i++)
        {
            value[i]=0;
        }

        AssignScore();// assign the score of a round to the UI
        interfaceManage.roundUI.text = "Round: " + staticScript.roundCounter.ToString();
        // assign the roundCounter value to the UI

        gameTypeImage = gameType.GetComponent<Image>();

        // Audio Shuffle
        if(staticScript.audioNumShuffled==false)
        {
            staticScript.audioNum = ShuffleArray(staticScript.audioNum);
            staticScript.audioNumShuffled = true;
        }
        //currentAudioNumber = staticScript.audioCounter;

        // gets the audio source everytime scene gets loaded

        instantiateAudio = new GameObject[ObjAudio.Length];
        backgroundAudio = new AudioSource[ObjAudio.Length];

        for (int i = 0; i < ObjAudio.Length; i++)
        {
            instantiateAudio[i] = Instantiate(ObjAudio[i], this.transform);
        }

        for (int i = 0; i < instantiateAudio.Length; i++)
        {
            backgroundAudio[i] = instantiateAudio[i].GetComponent<AudioSource>();
        }

        if(staticScript.audioStart==true)
        {
            AudioManagmentPlay();
        }
        
        // Ad startup
        Advertisement.Initialize(adID, true);

        if (staticScript.round1[0]!=0 || staticScript.round4[0]!=0)
        {
            interfaceManage.showScoreBoard();
            ShowAD();
        }
    }

    // Update is called once per frame
    void Update() {

        // For audio
        if (backgroundAudio[staticScript.audioNum[staticScript.audioCounter]].isPlaying==true)
        {
            staticScript.audioStart = true;
            PlayBtn.SetActive(false);
            PauseBtn.SetActive(true);
        }
        if(backgroundAudio[staticScript.audioNum[staticScript.audioCounter]].isPlaying==false && 
            staticScript.audioStart==true)// audioStart so only updates one time after paused
        {
            if (staticScript.audioCounter == 4)
            {
                staticScript.audioCounter = 0;
            }
            else
            {
                staticScript.audioCounter++;
            }
            //currentAudioNumber = staticScript.audioCounter;
            staticScript.audioStart = false;
        }

        time += Time.deltaTime;

        if (usedForMoveTowards == true) // placed above time > interpolation if so in
        // the last hit the player value will be assigned but it didnt work fixed using other method
        {
            JustMove();
            if (tempPosition == card1.transform.position)
            {
                usedForMoveTowards = false;
                tempPosition = Vector3.one;
                interfaceManage.AssignPlayersValue(value);// assign to the value UI when the cards reach
                // to the destination of the winner
                Destroy(card1);
                Destroy(card2);
                Destroy(card3);
                Destroy(card4);
                throwAfter = true;
            }
            else
            {
                tempPosition = card1.transform.position;
            }
        }

        if (time >= interpolationTime && waitAndRestart==true && animationControl==true
        &&waitTothrowUntilBidComplete==true && throwCount<=13)
        {
            time = time - interpolationTime;
            if(test[0]==true && throwAfter==true)
            {
                FirstPlayer();
                test[0]=false;
                test[1]=true;
                Counter++;
            }
            else if(test[1]==true && throwAfter==true)
            {
                SecondPlayer();
                test[1]=false;
                test[2]=true;
                Counter++;
            }
            else if(test[2]==true && throwAfter==true)
            {
                ThirdPlayer();
                test[2]=false;
                touchControl=true;
                Counter++;
                if(gameControl!=3)
                {
                    notAvailable = invalidForOneHitCards(ActiveCard, arrayCard1,activeCardForGameType);
                    lowerCards(notAvailable);
                }
                OneCardController =true;
            }
            else if(test[3]==true && throwAfter==true)
            {
                FourthPlayer();
                test[3]=false;
                test[0]=true;
                Counter++;
                touchControl=false;
            }
            if(Counter==4)
            {
                Counter=0;
                //Debug.Log("Boom");
                ActiveCard=-1; // to ensure it resets for every hit
                activeCardForGameType = 0;// to ensure it follows active card when it resets
                waitAndRestart=false;
                StartCoroutine(destroyEnemy());
                StartCoroutine(MoveDestroyMiddleCards());
                throwAfter=false;
                throwCount++;

                notAvailable = null;// if not nulled climb card will climb cards of the previous 
                // notavailable if the value is not deposed so will break the if null logic in ClimbCards

                for (int i = 0; i < test.Length; i++)
                {
                    if(winnerOfOneHit==3)
                    {
                        touchControl = true;// unlock the touch ability
                        OneCardController = true;
                        test[i] = false;
                    }
                    else
                    {
                        if (i == winnerOfOneHit)
                        {
                            test[i] = true;
                        }
                        else
                        {
                            test[i] = false;
                        }
                    }
                }
                gameControl = winnerOfOneHit;

                if(throwCount==13)
                {
                    throwCount++;
                    float[] flValue = new float[4]; // used for passing real time data of value because
                    // there is a problem with assignment of the player bid to ui in the last hit of a round
                    for (int i = 0; i < value.Length; i++)
                    {
                        flValue[i] = float.Parse(value[i].ToString());
                    }
                    StartCoroutine(oneRoundWinner(interfaceManage.returnBid(),flValue));
                    if(staticScript.roundCounter<6)
                    {
                        StartCoroutine(deleteAndLoad());
                    }
                }
            }
        }

        if(playerBidComplete==true)
        {
            bid[3]=staticScript.playerBid;
            playerBidComplete=false;
            waitTothrowUntilBidComplete=true;

            interfaceManage.AssignPlayersBid(bid);
        }
        
    }

    void FirstPlayer()
    {
        cardBackAnim=opponentCardBack[0].GetComponent<control_anim>();
		StartCoroutine(cardBackAnim.AnimateCard());
        arrayCard2=hitMeUp(sr0,arrayCard2,ActiveCard);
        //Debug.Log("First Opponent!");
    }
    void SecondPlayer()
    {
        cardBackAnim=opponentCardBack[1].GetComponent<control_anim>();
		StartCoroutine(cardBackAnim.AnimateCard());
        arrayCard3=hitMeUp(sr1,arrayCard3,ActiveCard);
        //Debug.Log("Second Opponent!");
    }
    void ThirdPlayer()
    {
        cardBackAnim=opponentCardBack[2].GetComponent<control_anim>();
		StartCoroutine(cardBackAnim.AnimateCard());
        arrayCard4=hitMeUp(sr2,arrayCard4,ActiveCard);
        //Debug.Log("Third Opponent!");
    }

    void FourthPlayer()
    {
        climbCards(notAvailable);
        //Debug.Log("Player Debug");
    }

    public int[] ShuffleArray(int[] numbers)
    {
        int[] newArray = numbers.Clone() as int[];
        for (int i = 0; i < newArray.Length; i++)
        {
            int tmp = newArray[i];
            int r = Random.Range(i, newArray.Length);
            newArray[i] = newArray[r];
            newArray[r] = tmp;
        }
        return newArray;
    }

    int[] fillArray(int[] arrayTobeUsed, int start)
    {
        int[] array = new int[13];

        for (int i = 0; i < 13; i++)
        {
            array[i] = arrayTobeUsed[start];
            start++;
        }
        return array;
    }

    int[] fillNumber()
    {
        int[] array = new int[52];
        for (int i = 0; i < 52; i++)
        {
            array[i] = i;
        }
        return array;
    }

    int[] bubbleSort(int[] arr)  // because its better than merge sort and many algorithms on small scale
    {
        bool control = true;
        for (int i = 0; i < arr.Length-1; i++)
        {
            if (arr[i]<=arr[i+1])
            {
                // does nothing
            }
            else
            {
                int temp = arr[i];
                arr[i] = arr[i + 1];
                arr[i + 1] = temp;
                control= false;
            }
            if (i == arr.Length - 2 && control == false)
            {
                i =-1;
                control = true;
            }

        }
        return arr;
    }

    float[] bubbleSort_float(float[] arr)  // because its better than merge sort and many algorithms on small scale
    {
        bool control = true;
        for (int i = 0; i < arr.Length-1; i++)
        {
            if (arr[i]<=arr[i+1])
            {
                // does nothing
            }
            else
            {
                float temp = arr[i];
                arr[i] = arr[i + 1];
                arr[i + 1] = temp;
                control= false;
            }
            if (i == arr.Length - 2 && control == false)
            {
                i =-1;
                control = true;
            }

        }
        return arr;
    }

    public void lowerCards(int[] notAllowed)
    {
        if(notAllowed==null)
        {
            // Null reference so does nothing
        }
        else
        {
            SpriteRenderer spriteRenderer=new SpriteRenderer();
            for (int i = 0; i < cards.Length; i++)
            {
                if(cards[i]!=null)// because it some of the cards in the array gets destroyed
                {
                    spriteRenderer=cards[i].GetComponent<SpriteRenderer>();
                    Sprite sprite=spriteRenderer.sprite;
                    int spriteNum=0;
                    for (int q = 0; q < cardImages.Length; q++)// used to identify the index of the current
                    // card sprite image which is matched with indexs of not allowed to identify which to lower
                    {
                        if(sprite.name==cardImages[q].name)
                        {
                                spriteNum=q;
                        }
                    }
                    for (int j = 0; j < notAllowed.Length; j++)
                    {
                        if(spriteNum==notAllowed[j])
                        {
                            if(cards[i]!=null)
                            {
                                Vector2 startpos = cards[i].transform.position;
                                cards[i].transform.position = new Vector2(startpos.x, startpos.y - 0.5f);
                            }
                        }   
                    }
                }
            }
        }
    }

    public void climbCards(int[] notAllowed)
    {
        if(notAllowed==null)
        {
            // Null reference so does nothing
        }
        else
        {
            SpriteRenderer spriteRenderer=new SpriteRenderer();
            for (int i = 0; i < cards.Length; i++)
            {
                if(cards[i]!=null)
                {
                    spriteRenderer=cards[i].GetComponent<SpriteRenderer>();
                    Sprite sprite=spriteRenderer.sprite;
                    int spriteNum=0;
                    for (int q = 0; q < cardImages.Length; q++)// used to identify the index of the current
                    // card sprite image which is matched with indexs of not allowed to identify which to lower
                    {
                        if(sprite.name==cardImages[q].name)
                        {
                                spriteNum=q;
                        }
                    }
                    for (int j = 0; j < notAllowed.Length; j++)
                    {
                        if(spriteNum==notAllowed[j])
                        {
                            if(cards[i]!=null)
                            {
                                Vector2 startpos = cards[i].transform.position;
                                cards[i].transform.position = new Vector2(startpos.x, startpos.y + 0.5f);
                            }
                        }   
                    }
                }
            }
        }
    }

    IEnumerator PlayerCardAnimation()
    {
        for (int i = 0; i < cards.Length; i++)
        {
            yield return new WaitForSeconds(0.15f);
            cards[i].SetActive(true);
        }
    }

    public void PlayerCardMoveAnimation(GameObject card)
    {
        card.transform.SetPositionAndRotation(DestinationPlace,Quaternion.identity);

        srPlayer=card.gameObject.GetComponent<SpriteRenderer>();
        disappear=true;

        addOneTimeClones(card);
    }

    public void scaleCard(GameObject touchedCard)
    {
        tempScale.x += 0.09f;
        tempScale.y += 0.09f;

        touchedCard.transform.localScale=tempScale;
    }

    public void UnscaleCard(GameObject touchedCard)
    {
        tempScale=originalScale;
        touchedCard.transform.localScale=tempScale;
    }

    public void addOneTimeClones(GameObject clone)
    {
        clonesCards.Add(clone);
    }

    IEnumerator MoveDestroyMiddleCards() // Add a vector3 variable for the cards to move
    {
        yield return new WaitForSeconds(0.3f);

        for (int i = 0; i < clonesCards.ToArray().Length; i++)
        {
            Destroy(clonesCards[i]);
        }
    }

    int[] invalidForOneHitCards(int cardNumber,int[] arrayCard,int gameType) // used to identify which card are allowed to be thrown
    {
        List<int> invalidCard=new List<int>();
        List<int> cardeXception=new List<int>();

        bool gameTypeCardExist=false,winPotentialExist=false,spadeExist=false,spadeWinPossible=false;

        int X=0,Y=0;
        if(gameType==0 || cardNumber==-1)
        {
            goto Skip_to_End;
        }
        else if(gameType<13)
        {
            X=0;
            Y=12;
        }
        else if(gameType<26)
        {
            X=13;
            Y=25;
        }
        else if(gameType<39)
        {
            X=26;
            Y=38;
        }
        else
        {
            X=39;
            Y=51;
        }
        // This function has one problem if it doesn't have any 
        // card greater than the activeCard/cardnumber it automatically goes to the spades while it should
        // allow the player to throw any card of the same type to the game :: HAS BEEN SOLVED

        for (int i = 0; i < arrayCard.Length; i++)
        {
            if(arrayCard[i]>=39)
            {
                spadeExist=true;
                if(arrayCard[i]>cardNumber)
                {
                    spadeWinPossible = true;
                }
            }
            if(arrayCard[i]>=X && arrayCard[i]<=Y)
            {
                cardeXception.Add(arrayCard[i]);
                gameTypeCardExist = true;
                if(arrayCard[i]>cardNumber)
                {
                    winPotentialExist = true;
                }
            }
        }
        if (gameTypeCardExist == true && winPotentialExist == true)
        {
            for (int i = 0; i < arrayCard.Length; i++)
            {
                bool exist = false;
                for (int j = 0; j < cardeXception.ToArray().Length; j++)
                {
                    if (arrayCard[i] == cardeXception[j])
                    {
                        if (arrayCard[i] > cardNumber)
                        {
                            exist = true; // allowed to be thrown
                        }
                    }
                }
                if (exist == false)
                {
                    invalidCard.Add(arrayCard[i]);
                }
            }
        }
        else if (gameTypeCardExist == true && winPotentialExist == false)
        {
            for (int i = 0; i < arrayCard.Length; i++)
            {
                bool exist = false;
                for (int j = 0; j < cardeXception.ToArray().Length; j++)
                {
                    if (arrayCard[i] == cardeXception[j])
                    {
                        exist = true;
                    }
                }
                if (exist == false)
                {
                    invalidCard.Add(arrayCard[i]);
                }
            }
        }
        else if (gameTypeCardExist == false && spadeExist == true && cardNumber >= 39)
        {
            if (spadeWinPossible == true)
            {
                for (int i = 0; i < arrayCard.Length; i++)
                {
                    if (arrayCard[i] > cardNumber)
                    {
                        // allowed to be thrown
                    }
                    else
                    {
                        invalidCard.Add(arrayCard[i]);
                    }
                }
            }
            else
            {
                // Every Card allowed because player can't win with her spade
            }
        }
        else if (gameTypeCardExist == false && spadeExist == true && cardNumber < 39)
        {
            for (int i = 0; i < arrayCard.Length; i++)
            {
                if (arrayCard[i] < 39)
                {
                    invalidCard.Add(arrayCard[i]);
                }
            }
        }
        else if (gameTypeCardExist == false && spadeExist == false)
        {
            // Does Nothing
        }
        Skip_to_End:
        return invalidCard.ToArray();
    }

    int IdentifyOneHitWinner(int gameCONTrol)
    {
        List<int> cloneCardNum=new List<int>();


        for (int j = 0; j < cardImages.Length; j++)
        {
            if(sr0.sprite.name==cardImages[j].name)
            {
                cloneCardNum.Add(j);
            }
        }
        for (int j = 0; j < cardImages.Length; j++)
        {
            if(sr1.sprite.name==cardImages[j].name)
            {
                cloneCardNum.Add(j);
            }
        }
        for (int j = 0; j < cardImages.Length; j++)
        {
            if(sr2.sprite.name==cardImages[j].name)
            {
                cloneCardNum.Add(j);
            }
        }

        for (int j = 0; j < cardImages.Length; j++)
        {
            if(srPlayer.sprite.name==cardImages[j].name)
            {
                cloneCardNum.Add(j);
            }
        }

        int hitCard;
        if(gameCONTrol==0)
        {
            hitCard=cloneCardNum[0];
        }
        else if(gameCONTrol==1)
        {
            hitCard=cloneCardNum[1];
        }
        else if(gameCONTrol==2)
        {
            hitCard=cloneCardNum[2];
        }
        else
        {
            hitCard=cloneCardNum[3];
        }

        int X=0,Y=0;

        if(hitCard<13)
        {
            X=0;
            Y=12;
        }
        else if(hitCard<26)
        {
            X=13;
            Y=25;
        }
        else if(hitCard<39)
        {
            X=26;
            Y=38;
        }
        else
        {
            X=39;
            Y=51;
        }

        int[] array=cloneCardNum.ToArray();
        for(int i=0; i < array.Length; i++)
        {
            if(array[i]>=X && array[i]<=Y)
            {
                // it is in the same card category as the hit means if the hit card is flower 
                // this card is also flower
            }
            else if(array[i]>=39)
            {
                // this means its spade and spade can overpass other cards
            }
            else
            {
                array[i]=0;
            }
        }
        int maximum=0;
        int maxPos=0;

        for (int i = 0; i < array.Length; i++)
        {
            if(array[i]>maximum)
            {
                maximum=array[i];
                maxPos=i;
            }
        }
        value[maxPos]++;
        return maxPos;   
    }

    public void assignActiveCard(int activeCard)
    {
        if(activeCardForGameType==0)
        {
            activeCardForGameType = activeCard;
        }
        if(activeCard>ActiveCard)
        {
            ActiveCard=activeCard;
            assignGameType(activeCardForGameType);
        }
    }

    int[] hitMeUp(SpriteRenderer sr,int[] arrayCard,int activeCard)
    {
        int[] notAllowed=invalidForOneHitCards(activeCard,arrayCard,activeCardForGameType);
		List<int> AllowedCard=new List<int>();
		bool breakCount=false;

		if(notAllowed.Length==0)
		{
			for (int i = 0; i < arrayCard.Length; i++)
			{
                if(arrayCard[i]!=-1)
                {
                    AllowedCard.Add(arrayCard[i]);
                }
			}
		}
		else
		{
			for (int i = 0; i < arrayCard.Length; i++)
			{
				for (int j = 0; j < notAllowed.Length; j++)
				{
					if(arrayCard[i]==notAllowed[j])
					{
						breakCount=true;
					}
				}
                if(breakCount==false)
				{
					AllowedCard.Add(arrayCard[i]);
				}
                else
                {
                    breakCount=false;
                }
			}
		}

		int[] allowedArray=AllowedCard.ToArray();
        int cardId = AIBotSimulator(allowedArray, activeCard);

        if(allowedArray.Length!=0)
        {
            sr.sprite=cardImages[allowedArray[cardId]]; /* gets the sprite rendrer of the current 
            game object and set the desired sprite out of the sprite registered to the game object*/

            assignActiveCard(allowedArray[cardId]);
            for (int i = 0; i<arrayCard.Length; i++) // make sure used card become obsolete by making it -1
            {
                if(allowedArray[cardId]==arrayCard[i])
                {
                    arrayCard[i]=-1;
                }
            }
        }
        else
        {
            if(notAllowed.Length==0)
            {
                //Debug.Log("All cards Have been selected");
            }
        }
        return arrayCard;
    }

    IEnumerator destroyEnemy()
    {
        winnerOfOneHit=IdentifyOneHitWinner(gameControl);
        //Debug.Log("Winner is player: "+winnerOfOneHit);
         
        card1=Instantiate(cardDropPlace[0],cardDropPlace[0].transform.position
        ,Quaternion.Euler(cardDropPlace[0].transform.eulerAngles));

        card2=Instantiate(cardDropPlace[1],cardDropPlace[1].transform.position
        ,Quaternion.Euler(cardDropPlace[1].transform.eulerAngles));
        
        card3=Instantiate(cardDropPlace[2],cardDropPlace[2].transform.position
        ,Quaternion.Euler(cardDropPlace[2].transform.eulerAngles));

        card4=Instantiate(cardDropPlace[3],cardDropPlace[3].transform.position
        ,Quaternion.Euler(eulerPlayerAngles));


        SpriteRenderer SprE1=card1.GetComponent<SpriteRenderer>();
        SpriteRenderer SprE2=card2.GetComponent<SpriteRenderer>();
        SpriteRenderer SprE3=card3.GetComponent<SpriteRenderer>();
        SpriteRenderer SprE4=card4.GetComponent<SpriteRenderer>();

        Sprite s1,s2,s3,s4;
        s1=sr0.sprite;
        s2=sr1.sprite;
        s3=sr2.sprite;
        s4=srPlayer.sprite;
        
        if(disappear==true)
        {
            sr0.sprite=null;
            sr1.sprite=null;
            sr2.sprite=null;
            disappear=false;
        }

        SprE1.sprite=s1;
        SprE2.sprite=s2;
        SprE3.sprite=s3;
        SprE4.sprite=s4;

        SprE1.sortingOrder=3;
        SprE2.sortingOrder=2;
        SprE3.sortingOrder=3;
        SprE4.sortingOrder=4; // set their sorting order so they won't be overlayed each on each other
        // as they are being created which maintains the visual order as was intended

        yield return new WaitForSeconds(1.0f);

        waitAndRestart=true;
        usedForMoveTowards=true;
        tempPosition=card1.transform.position;
    }

    
    void JustMove()
    {
        card1.transform.position=Vector3.MoveTowards(card1.transform.position
        ,otherPlayerCard[winnerOfOneHit].transform.position,speed*Time.deltaTime);

        card2.transform.position=Vector3.MoveTowards(card2.transform.position
        ,otherPlayerCard[winnerOfOneHit].transform.position,speed*Time.deltaTime);

        card3.transform.position=Vector3.MoveTowards(card3.transform.position
        ,otherPlayerCard[winnerOfOneHit].transform.position,speed*Time.deltaTime);

        card4.transform.position=Vector3.MoveTowards(card4.transform.position
        ,otherPlayerCard[winnerOfOneHit].transform.position,speed*Time.deltaTime);

    }

    public IEnumerator deleteAndLoad()
    {
        Destroy(opponentCardBack[0]);
        yield return new WaitForSeconds(0.3f);
        Destroy(opponentCardBack[1]);
        yield return new WaitForSeconds(0.3f);
        Destroy(opponentCardBack[2]);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(1);// after every card is thrown delete and reload the scene
    }

    IEnumerator oneRoundWinner(float[] cardsBid,float[] cardsValue) // assign the score of each player
    // by analyzing the players value and bid and assign the score of each player which can be negative or positive
    // depending whether the player have met his bid or not and assign the value to the static script which
    // is used to hold data when the scene reload and used by AssignScore function to assign it to UI
    {
        float[] roundScore = new float[4];

        for (int i = 0; i < cardsBid.Length; i++)
        {
            if(cardsValue[i]>=cardsBid[i])
            {
                float difference=cardsValue[i]-cardsBid[i];
                string temp = cardsBid[i].ToString() + "." + difference.ToString();
                roundScore[i]=float.Parse(temp);
            }
            else
            {
                string temp = "-" + cardsBid[i].ToString();
                roundScore[i] = float.Parse(temp);
            }
        }

        if (staticScript.roundCounter ==5) // placed here so this can happen before the waitforseconds line
            // gets called down below
        {
            staticScript.roundCounter++;
        }

        // assign round value to the respective variable in the static script to be
        // assigned to the scoreboard text when the scene is reloaded

        if (staticScript.roundCounter==1) 
        {
            staticScript.round1[0]=roundScore[0];
            staticScript.round1[1]=roundScore[1];
            staticScript.round1[2]=roundScore[2];
            staticScript.round1[3]=roundScore[3];
        }
        else if(staticScript.roundCounter==2)
        {
            staticScript.round2[0]=roundScore[0];
            staticScript.round2[1]=roundScore[1];
            staticScript.round2[2]=roundScore[2];
            staticScript.round2[3]=roundScore[3];
        }
        else if(staticScript.roundCounter==3)
        {
            staticScript.round3[0]=roundScore[0];
            staticScript.round3[1]=roundScore[1];
            staticScript.round3[2]=roundScore[2];
            staticScript.round3[3]=roundScore[3];
        }
        else if(staticScript.roundCounter==4)
        {
            staticScript.round4[0]=roundScore[0];
            staticScript.round4[1]=roundScore[1];
            staticScript.round4[2]=roundScore[2];
            staticScript.round4[3]=roundScore[3];
        }
        else
        {
            staticScript.round5[0]=roundScore[0];
            staticScript.round5[1]=roundScore[1];
            staticScript.round5[2]=roundScore[2];
            staticScript.round5[3]=roundScore[3];
            
            interfaceManage.showScoreBoard();

            interfaceManage.AssignScoreBoard();
            yield return new WaitForSeconds(0.35f);
            CalculateFinalResult();
        }
        if(staticScript.roundCounter<5)// because if placed above like the one before round1 will never
            // be assigned
        {
            staticScript.roundCounter++;
            interfaceManage.roundUI.text = "Round: " + staticScript.roundCounter.ToString();
        }
    }
    void AssignScore() // assign score to the score board text the value of the previous round at the start
    // of the current match so if the player click the trophy sign he will see the result of his plays
    {
        interfaceManage.AssignScoreBoard();
    }
    void CalculateFinalResult() // calculate final result, rand and assign them to the score board text
    {
        float[] finalResult=new float[4];
        string[] winner=new string[4];

        for (int i = 0; i < 4; i++)
        {
            finalResult[i]=staticScript.round1[i]+staticScript.round2[i]+
            staticScript.round3[i]+staticScript.round4[i]+staticScript.round5[i];
        }
        
        float[] posHolder=new float[4];
        // posHolder=finalResult; learning moment if you assign like this your actually assigning a reference
        // so if finalResult change its value posHolder Will also change its value learned that the hardWAY!!!
        for (int i = 0; i < posHolder.Length; i++)
        {
            posHolder[i] = finalResult[i];
        }
        int[] rank=new int[4];

        finalResult=bubbleSort_float(finalResult);

        for (int i = 0; i < posHolder.Length; i++) // identify the rank of the players
        {
            for (int j = 0; j < finalResult.Length; j++)
            {
                if(posHolder[i]==finalResult[j])
                {
                    rank[i]=j;
                }
            }
            if(rank[i]==0)
            {
                winner[i]="Fourth ";
            }
            else if(rank[i]==1)
            {
                winner[i]="Third";
            }
            else if(rank[i]==2)
            {
                winner[i]="Second";
            }
            else
            {
                winner[i]="Winner";
            }
        }
        interfaceManage.WinnerChickenDinner(winner,posHolder);
    }

    private int bidGuess(int[] arrayOfCard) // determines how much king and A's of each type of card 
    // the player has and make a guess for the bid 
    {
        float point = 0;
        int counter = 0;

        for (int i = 0; i < arrayOfCard.Length; i++)
        {
            if (arrayOfCard[i] < 13)
            {
                if (arrayOfCard[i] == 12)
                {
                    point++;
                }
                else if (arrayOfCard[i] >= 10)
                {
                    counter++;
                }
            }
            else if (arrayOfCard[i] < 26)
            {
                if (arrayOfCard[i] == 25)
                {
                    point++;
                }
                else if (arrayOfCard[i] >= 23)
                {
                    counter++;
                }
            }
            else if (arrayOfCard[i] < 39)
            {
                if (arrayOfCard[i] == 38)
                {
                    point++;
                }
                else if(arrayOfCard[i]>=36)
                {
                    counter++;
                }
            }
            else if (arrayOfCard[i] < 52)
            {
                if(arrayOfCard[i]>=48)
                {
                    point++;
                }
                else
                {
                    counter++;
                }
            }
        }
        //Debug.Log("Counter is: " + counter);
        if(counter<=3)
        {
            point += 1;
        }
        else
        {
            point += 2;
        }
        return int.Parse(point.ToString()); 
    }
    void assignGameType(int activEcard)
    {
        if(activEcard<13)
        {
            gameTypeImage.sprite = gameTypeSprite[0];
        }
        else if(activEcard<26)
        {
            gameTypeImage.sprite = gameTypeSprite[1];
        }
        else if(activEcard<39)
        {
            gameTypeImage.sprite = gameTypeSprite[2];
        }
        else
        {
            gameTypeImage.sprite = gameTypeSprite[3];
        }
    }
    int AIBotSimulator(int[] allowedArray,int activeCard)
    {
        int returnvar = -1;
        int lastValuePos = allowedArray.Length - 1;
        
        if(activeCard>allowedArray[lastValuePos])
        {
            List<int> eligibleForDispoal = new List<int>();
            for (int i = 0; i < allowedArray.Length; i++)
            {
                if(allowedArray[i]<=8)
                {
                    eligibleForDispoal.Add(allowedArray[i]);
                }
                else if(allowedArray[i]>=13 && allowedArray[i] <= 21)
                {
                    eligibleForDispoal.Add(allowedArray[i]);
                }
                else if(allowedArray[i]>=26 && allowedArray[i]<=34)
                {
                    eligibleForDispoal.Add(allowedArray[i]);
                }
            }
            int[] eligible = eligibleForDispoal.ToArray();
            
            if(eligible.Length==0)
            {
                returnvar = 0;
            }
            else
            {
                for (int i = 0; i < allowedArray.Length; i++)
                {
                    if(eligible[0]==allowedArray[i])
                    {
                        returnvar = i;
                        break;
                    }
                }
            }
        }
        else
        {
            for (int i = lastValuePos; i >= 0; i--)
            {
                if (allowedArray[i] < 13 && allowedArray[i] >= 10)
                {
                    if (returnvar == -1)
                    {
                        returnvar = i;
                        break;
                    }
                    else if (allowedArray[returnvar] > allowedArray[i] && allowedArray[returnvar] >= 13)
                    {
                        returnvar = i;
                        break;
                    }
                }
                else if (allowedArray[i] < 26 && allowedArray[i] >= 23)
                {
                    if (returnvar == -1)
                    {
                        returnvar = i;
                    }
                    else if (allowedArray[returnvar] > allowedArray[i] && allowedArray[returnvar] >= 26)
                    {
                        returnvar = i;
                    }
                }
                else if (allowedArray[i] < 39 && allowedArray[i] >= 36)
                {
                    if (returnvar == -1)
                    {
                        returnvar = i;
                    }
                }
            }
            if (returnvar == -1) // means the player has a chance of winning and not the first to throw for
            // this hit or doesnt have a high class card in each game type lower than spade
            {
                for (int i = 0; i < allowedArray.Length; i++)
                {
                    if (allowedArray[i] < 13)
                    {
                        returnvar = i;
                    }
                    else if (allowedArray[i] < 26)
                    {
                        if (returnvar == -1)
                        {
                            returnvar = i;
                        }
                        else if (returnvar != -1 && allowedArray[returnvar] >= 13)
                        {
                            returnvar = i;
                        }
                        else
                            break;
                    }
                    else if (allowedArray[i] < 39)
                    {
                        if (returnvar == -1)
                        {
                            returnvar = i;
                        }
                        else if(returnvar != -1 && returnvar >= 26)
                        {
                            returnvar = i;
                        }
                        else
                            break;
                    }
                }
            }
        }
        if(returnvar==-1)// only Spades are allowed
        {
            returnvar = 0;
        }
        return returnvar;
    }

    public void AudioManagmentPlay()
    {
        backgroundAudio[staticScript.audioNum[staticScript.audioCounter]].Play();
        PlayBtn.SetActive(false);
        PauseBtn.SetActive(true);
    }
    public void AudioManagmentPause()
    {
        backgroundAudio[staticScript.audioNum[staticScript.audioCounter]].Stop();
        PlayBtn.SetActive(true);
        PauseBtn.SetActive(false);
    }
    public void ShowAD()
    {
        Advertisement.Show();
    }
}