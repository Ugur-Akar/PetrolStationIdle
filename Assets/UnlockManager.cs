using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UnlockManager : MonoBehaviour
{
    //Settings

    // Connections
    public List<Unlock> locks;

    public event Action<GameObject> UnlockedEvent;
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

    public void SetLocks(List<Unlock> lockList)
    {
        
        for(int i = 0; i < lockList.Count; i++)
        {
            locks.Add(lockList[i]);
            locks[i].Unlocked += Unlocked;
        }

    }
    

    void Unlocked(GameObject go)
    {
        UnlockedEvent(go);
    }

}

