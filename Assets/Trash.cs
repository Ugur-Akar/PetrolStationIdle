using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Trash : MonoBehaviour
{
    //Settings

    // Connections
    public DOTweenAnimation scaleable;
    // State Variables
    bool tweenIsPlaying = false;
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
        if (other.CompareTag("Player"))
        {
            scaleable.DORestart();
            tweenIsPlaying = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && !tweenIsPlaying)
        {
            scaleable.DORestart();
            tweenIsPlaying = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scaleable.DOPause();
            tweenIsPlaying = false;
        }
    }
}

