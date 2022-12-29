using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class MainSource : MonoBehaviour
{
    //Settings
    public bool isOil = false;
    public bool underInv = false;
    // Connections
    public event Action<bool> MainSourceUnlocked;
    public Action<bool> xAction;
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

    private void OnEnable()
    {
        MainSourceUnlocked?.Invoke(isOil);
    }
}

