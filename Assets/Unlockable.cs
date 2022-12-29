using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Unlockable : MonoBehaviour
{
    //Settings

    // Connections
    public DOTweenAnimation[] unlockTweens;
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

    public void PlayTweens()
    {
        for(int i = 0; i < unlockTweens.Length; i++)
        {
            unlockTweens[i].DOPlay();
        }
    }
}

