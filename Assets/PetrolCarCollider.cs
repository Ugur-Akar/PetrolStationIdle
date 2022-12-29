using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PetrolCarCollider : MonoBehaviour
{
    //Settings

    // Connections
    public Transform parentTransform;

    public event Action<GameObject> PumpStopEvent;
    public event Action CarEnter;
    public event Action CarExit;
    public event Action PlayerEnter;
    public event Action PlayerExit;
    // State Variables
    
    // Start is called before the first frame update
	void Awake()
	{
		//InitConnections();
	}
    void Start()
    {
        //InitState();
    }
    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PumpStop"))
        {
            PumpStopEvent(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerEnter();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerExit();
        }
    }


    public void CallCarEnter()
    {
        CarEnter();
    }

    public void CallCarExit()
    {
        CarExit();
    }
}

