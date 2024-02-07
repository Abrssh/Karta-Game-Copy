using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityScript;

public class control_anim : MonoBehaviour {

    Animator anim;

    Vector3 secondPlayer=new Vector3(0,0,-43.883f);
    Vector3 thirdPlayer=new Vector3(0.167f,-0.576f,-222.06f);
    Vector3 fourthPlayer=new Vector3(-4.866f,-0.654f,-128.56f);

    [SerializeField] GameObject[] otherPlayers;
    
    [SerializeField] Management management;

    Vector3 defaultRotation;

    // use bool for animation synchronization

    // Use this for initialization
    void Start() {

        anim = GetComponent<Animator>();
        StartCoroutine(jumbToWave());
        anim.SetBool("waveToOut", true);
    }

    // Update is called once per frame
    void Update() {

    }

    IEnumerator jumbToWave()
    {
        yield return new WaitForSeconds(2.2f);
        if(otherPlayers[0].name==this.transform.name)
        {
            defaultRotation=transform.rotation.eulerAngles;
            transform.rotation= Quaternion.Euler(secondPlayer);
        }
        else if(otherPlayers[1].name==this.transform.name)
        {
            defaultRotation=transform.rotation.eulerAngles;
            transform.rotation=Quaternion.Euler(thirdPlayer);
        }
        else
        {
            defaultRotation=transform.rotation.eulerAngles;
            transform.rotation=Quaternion.Euler(fourthPlayer);
        }
        anim.SetBool("jumbleToWave",true);
        yield return new WaitForSeconds(1.63f);
        transform.rotation=Quaternion.Euler(defaultRotation);
        management.animationControl=true;
    }

    public IEnumerator AnimateCard()
    {
        anim.SetBool("outIn",true);
        yield return new WaitForSeconds(0.0f); // 0.17f
        anim.SetBool("outIn",false);
    }
}

