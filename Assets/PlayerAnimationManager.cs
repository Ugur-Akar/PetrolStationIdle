using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationManager : MonoBehaviour
{
    //Settings

    // Connections
    Animator anim;
    // State Variables
    bool isWalking = false;
    bool isCarrying = false;
    // Start is called before the first frame update
	void Awake()
	{
		InitConnections();
	}
    void Start()
    {
        //InitState();
    }
    void InitConnections(){
        anim = GetComponent<Animator>();
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Walk()
    {
        anim.SetTrigger("Walk");
    }
    public void Idle()
    {
        anim.SetTrigger("Idle");
    }

    public void EnableHasItem()
    {
        anim.SetBool("HasItem", true);
    }

    public void DisableHasItem()
    {
        anim.SetBool("HasItem", false);
    }


    public void CarryWalk()
    {
        if (!isWalking || !isCarrying)
        {
            isWalking = true;
            isCarrying = true;
            anim.SetTrigger("CarryWalk");
        }
    }

    public void EmptyWalk()
    {
        if(!isWalking || isCarrying)
        {
            isWalking = true;
            isCarrying = false;
            anim.SetTrigger("EmptyWalk");
        }
    }

    public void CarryIdle()
    {
        if(isWalking || !isCarrying)
        {
            isWalking = false;
            isCarrying = true;
            anim.SetTrigger("CarryIdle");
        }
    }

    public void EmptyIdle()
    {
        if(isWalking || isCarrying)
        {
            isWalking = false;
            isCarrying = false;
            anim.SetTrigger("EmptyIdle");
        }
    }
}

