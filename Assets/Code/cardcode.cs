using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardcode : MonoBehaviour {

    // Use this for initialization
    public Management manage;
    Collider2D collid,touchedCollider;
    bool movedAllowed = false;
    float yPosition; // hold initial value of the game object y position

    int differenceControl=0; // used to indicate whether the card has been move enough to be thrown
    // which is used to determine if the card should be put back into its place or thrown to the middle
    // and determine wheter the card in question has been touched or not to not put every card to their
    // default position

    //GameObject attachedCard;

    void Start () {

        collid = GetComponent<Collider2D>();

        yPosition=this.transform.position.y;
	}

	// Update is called once per frame
	void Update () {
        if(Input.touchCount>0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

            if(touch.phase==TouchPhase.Began && manage.touchControl==true )
            {
                if(transform.position.y==Management.defaultYCardPositon) // checks if the card has been lowered
                {
                    touchedCollider= Physics2D.OverlapPoint(touchPosition);
                    if (collid == touchedCollider)
                    {
                        movedAllowed = true;
                        manage.scaleCard(touchedCollider.gameObject);
                        differenceControl=1; // indicate it has been touched
                    }
                }
            }
            if(touch.phase==TouchPhase.Moved && manage.OneCardController==true)
            {
                if(movedAllowed==true && touchPosition.y>transform.position.y)
                {
                    transform.position = new Vector2(transform.position.x,touchPosition.y);
                    float difference=touchPosition.y-yPosition;
                    if(difference>=1f)
                    {
                        manage.PlayerCardMoveAnimation(this.gameObject);
                        manage.test[3]=true;
                        differenceControl=2;

                        SpriteRenderer spriteRenderer=new SpriteRenderer();
                        spriteRenderer=this.transform.GetComponent<SpriteRenderer>();
                        string spriteName=spriteRenderer.sprite.name;
                        
                        for (int i = 0; i < manage.cardImages.Length; i++)
                        {
                            if(manage.cardImages[i].name==spriteName)
                            {
                                //Debug.Log("Active Card from CardCode : "+i);
                                manage.assignActiveCard(i);
                                for (int t = 0; t < manage.arrayCard1.Length; t++)
                                {
                                    if(i==manage.arrayCard1[t])
                                    {
                                        manage.arrayCard1[t]=-1;
                                    }
                                }
                                break;
                            }
                        }
                        manage.OneCardController=false;
                    }
                }
            }
            if(touch.phase==TouchPhase.Ended)
            {
                movedAllowed = false;
                if(differenceControl==1)
                {
                    transform.position=new Vector2(transform.position.x,yPosition);
                    if(touchedCollider!=null)
                    {
                        manage.UnscaleCard(touchedCollider.gameObject);
                    }
                }
                else if(differenceControl==2)
                {
                    if(touchedCollider!=null)
                    {
                        manage.UnscaleCard(touchedCollider.gameObject);
                    }
                }
            }
        }
	}

}
