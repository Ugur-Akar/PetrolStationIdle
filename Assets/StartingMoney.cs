using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StartingMoney : MonoBehaviour
{
    //Settings
    public bool isTask = true;
    public int amountOfMoneyToActivate = 80;
    // Connections
    TaskManager taskManager;
    ObjectPool op;
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

    public void ActivateMoney()
    {
        Debug.Log("Sm:Starts");
        GetComponent<Collider>().enabled = true;
        taskManager = GetComponent<TaskManager>();
        op = GetComponent<ObjectPool>();

        op.ActivateStartingMoney(amountOfMoneyToActivate);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            taskManager.TaskCompletedAndEnabled();
            GetComponent<Collider>().enabled = false;
        }
    }
}

