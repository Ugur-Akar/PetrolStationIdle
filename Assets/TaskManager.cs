using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TaskManager : MonoBehaviour
{
    //Settings
    // Connections
    public event Action TaskDone;
    // State Variables
    public bool isCurrentTask = false;
    
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

    public void TaskCompletedAndDisable()
    {
        if (isCurrentTask)
        {
            TaskDone();
            isCurrentTask = false;
            gameObject.SetActive(false);
        }
        
    }

    public void TaskCompletedAndEnabled()
    {
        if (isCurrentTask)
        {
            TaskDone();
            isCurrentTask = false;
        }
    }
}

