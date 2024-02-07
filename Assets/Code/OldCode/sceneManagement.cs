using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneManagement : MonoBehaviour {

    [SerializeField] private Sprite[] cardImages;
    [SerializeField] private GameObject[] cards;

    [SerializeField] private GameObject[] cardDropPlace; // replace with the positions of the gameObjects

    [SerializeField] private GameObject[] opponentCardBack;

    [SerializeField] private GameObject[] otherPlayerCard; // replace with the positions of the gameObjects

    int[] arrayforCards = new int[52];
    int[] arrayCard1, arrayCard2, arrayCard3, arrayCard4,testArray;

    Vector3 tempScale=new Vector3(0.2466041f,0.2616924f,0.2466041f);
    Vector3 originalScale=new Vector3(0.2466041f,0.2616924f,0.2466041f);

     Vector3 DestinationPlace;

     bool[] gameControl={false,false,false};

     GameObject[] clonesCards=new GameObject[4];

    float time=0,interpolationTime=0.3f;
    // Use this for initialization

    control_anim cardBackAnim;

    Vector3[] startPosForOtherPlayerCard=new Vector3[4];
    Vector3[] cardDropPositons=new Vector3[4];

    int controlForaddClone=0;
    
    bool moveDestroyControl=false,oneTimeWait=true; // used to control when to be called in update and
    // when to wait for some time rather than waiting in every update call
    public bool lowerUpperCard=true,lowered=false; // used to control the lowering and Climbing of cards and
    //lowered controls whether the cards are lowered or not
    int[] notAllowedNum;


    void Start()
    {
        arrayforCards = fillNumber();
        arrayforCards = ShuffleArray(arrayforCards);

        arrayCard1 = fillArray(arrayforCards, 0);
        arrayCard2 = fillArray(arrayforCards, 13);
        arrayCard3 = fillArray(arrayforCards, 26);
        arrayCard4 = fillArray(arrayforCards, 39);

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

        /*testArray=new int[arrayCard1.Length-8];

        for (int i = 0; i < arrayCard1.Length-8; i++)
        {
            testArray[i]=arrayCard1[i]; 
        }*/

        startPosForOtherPlayerCard[0]=otherPlayerCard[0].transform.position;
        startPosForOtherPlayerCard[1]=otherPlayerCard[1].transform.position;
        startPosForOtherPlayerCard[2]=otherPlayerCard[2].transform.position;
        startPosForOtherPlayerCard[3]=otherPlayerCard[3].transform.position;

        cardDropPositons[0]=cardDropPlace[0].transform.position;
        cardDropPositons[1]=cardDropPlace[1].transform.position;
        cardDropPositons[2]=cardDropPlace[2].transform.position;
        cardDropPositons[3]=cardDropPlace[3].transform.position;
    }

    // Update is called once per frame
    void Update() {

        time += Time.deltaTime;

        if (time >= interpolationTime)
        {
            time = time - interpolationTime;
            int test=Random.Range(0,51);
            Debug.Log("Test Value is: "+test);
            notAllowedNum=invalidForOneHitCards(test);
            for (int i = 0; i < notAllowedNum.Length; i++)
            {
                Debug.Log("Not Allowed: "+notAllowedNum[i]);
            }
            if(lowerUpperCard==true && lowered==false)
            {
                lowerCards(notAllowedNum);
            }
            else if(lowerUpperCard==false && lowered==true)
            {
                climbCards(notAllowedNum);
            }

            if(gameControl[0]==true)
            {
                cardBackAnim=opponentCardBack[0].GetComponent<control_anim>();
                StartCoroutine(cardBackAnim.AnimateCard());
                otherPlayerCardMovement(startPosForOtherPlayerCard[0],5
                ,arrayCard2,2,0);
                gameControl[0]=false;
            }
            else if(gameControl[1]==true)
            {
                cardBackAnim=opponentCardBack[1].GetComponent<control_anim>();
                StartCoroutine(cardBackAnim.AnimateCard());
                gameControl[1]=false;
                otherPlayerCardMovement(startPosForOtherPlayerCard[1],5
                ,arrayCard3,0,1);
            }
            else if(gameControl[2]==true)
            {
                cardBackAnim=opponentCardBack[2].GetComponent<control_anim>();
                StartCoroutine(cardBackAnim.AnimateCard());
                gameControl[2]=false;
                otherPlayerCardMovement(startPosForOtherPlayerCard[2],5
                ,arrayCard2,1,2);
                moveDestroyControl=true;
                oneTimeWait=true;
            }
        }
        if(moveDestroyControl==true)
        {
            int i=IdentifyOneHitWinner(clonesCards);
            Debug.Log("I is+ "+i);
            StartCoroutine(MoveDestroyMiddleCards(startPosForOtherPlayerCard[i]));
        }
    }

    private int[] ShuffleArray(int[] numbers)
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

    public void lowerCards(int[] notAllowed)
    {
        for (int i = 0; i < arrayCard1.Length; i++)
        {
            for (int j = 0; j < notAllowed.Length; j++)
            {
                if(i==notAllowed[j])
                {
                    if(cards[i]!=null)
                    {
                        Vector2 startpos = cards[i].transform.position;
                        cards[i].transform.position = new Vector2(startpos.x, startpos.y - 0.8f);
                    }
                }   
            }
        }
        lowered=true;
    }

    public void climbCards(int[] notAllowed)
    {
       for (int i = 0; i < cards.Length; i++)
        {
            for (int j = 0; j < notAllowed.Length; j++)
            {
                if(i==notAllowed[j])
                {
                    if(cards[i]!=null)
                    {
                        Vector2 startpos = cards[i].transform.position;
                        cards[i].transform.position = new Vector2(startpos.x, startpos.y + 0.8f);
                    }
                }   
            }
        }
        lowered=false;
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
        Destroy(cardDropPlace[3]);
        for (int i = 0; i < gameControl.Length; i++)
        {
            gameControl[i]=true;
        }
        addOneTimeClones(card);
    }

    public void scaleCard(GameObject touchedCard)
    {
        tempScale.x += 0.05f;
        tempScale.y += 0.05f;

        touchedCard.transform.localScale=tempScale;
    }

    public void UnscaleCard(GameObject touchedCard)
    {
        tempScale=originalScale;
        touchedCard.transform.localScale=tempScale;
    }

    void otherPlayerCardMovement(Vector3 startPos,int cardId,int[] arrayCard,int dropPlace,int plCard)
    {
        SpriteRenderer spriteR=new SpriteRenderer();
        GameObject currentCard=Instantiate(otherPlayerCard[plCard],startPos
        ,Quaternion.Euler(otherPlayerCard[plCard].transform.eulerAngles));// instantiate
        //game object
        
        cardId=Random.Range(0,10);
        
        spriteR=currentCard.GetComponent<SpriteRenderer>();
        spriteR.sprite=cardImages[arrayCard[cardId]]; // gets the sprite rendrer of the current game object and
        // set the desired sprite out of the sprite registered to the game object

        currentCard.transform.SetPositionAndRotation(cardDropPositons[dropPlace],
        Quaternion.Euler(otherPlayerCard[plCard].transform.eulerAngles));

        addOneTimeClones(currentCard);
    }

    public void addOneTimeClones(GameObject clone)
    {
        clonesCards[controlForaddClone]=clone;
        controlForaddClone++;
        if(controlForaddClone>=4)
        {
            controlForaddClone=0;
        }
    }

    IEnumerator MoveDestroyMiddleCards(Vector3 moveTo) // Add a vector3 variable for the cards to move
    {
        float speed=4.0f;

        Vector3 tempCheck=clonesCards[0].transform.position;

        if(oneTimeWait==true)
        {
            yield return new WaitForSeconds(1f);
            oneTimeWait=false;
        }

        clonesCards[0].transform.position=Vector3.MoveTowards(clonesCards[0].transform.position
        ,moveTo,speed*Time.deltaTime);
        clonesCards[1].transform.position=Vector3.MoveTowards(clonesCards[1].transform.position
        ,moveTo,speed*Time.deltaTime);
        clonesCards[2].transform.position=Vector3.MoveTowards(clonesCards[2].transform.position
        ,moveTo,speed*Time.deltaTime);
        clonesCards[3].transform.position=Vector3.MoveTowards(clonesCards[3].transform.position
        ,moveTo,speed*Time.deltaTime);

        if(tempCheck==clonesCards[0].transform.position)
        {
            moveDestroyControl=false;
            for (int i = 0; i < clonesCards.Length; i++)
            {
                yield return new WaitForSeconds(0.2f);
                Destroy(clonesCards[i]);
            }
        }
        lowerUpperCard=true;
    }

    int[] invalidForOneHitCards(int cardNumber) // used to identify which card are allowed to be thrown
    {
        List<int> invalidCard=new List<int>();

        int X=0,Y=0;

        if(cardNumber<13)
        {
            X=0;
            Y=12;
        }
        else if(cardNumber<26)
        {
            X=13;
            Y=25;
        }
        else if(cardNumber<39)
        {
            X=26;
            Y=38;
        }
        else
        {
            X=39;
            Y=51;
        }

        for (int i = 0; i < arrayCard1.Length; i++)
        {
            if(arrayCard1[i]<cardNumber && arrayCard1[i]>X && arrayCard1[i]<Y)
            {
                invalidCard.Add(i);
            }
        }
        return invalidCard.ToArray();
    }

    int IdentifyOneHitWinner(GameObject[] clones)
    {
        List<int> cloneCardNum=new List<int>();

        for (int i = 0; i < clones.Length; i++)
        {
            for (int j = 0; j < cardImages.Length; j++)
            {
                if(clones[i].name==cardImages[j].name)
                {
                    cloneCardNum.Add(j);
                }
            }
        }

        int[] array=cloneCardNum.ToArray();
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
        return maxPos;   
    }
}