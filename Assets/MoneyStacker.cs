using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoneyStacker : MonoBehaviour
{
    //Settings
    int maxAmount;
    // Connections
    public ObjectPool objectPool;
    public List<Money> moneyList;
    public List<Vector3> positions;
    public GameObject[] objects;
    // State Variables
    
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
        maxAmount = objectPool.amountToPool;
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PayMoney(int amount, Transform car)
    {
        GameObject newMoneyGO = objectPool.GetPooledObject();
        if (newMoneyGO != null)
        {
            newMoneyGO.transform.position = car.position;
            Money money = newMoneyGO.GetComponent<Money>();
        }
    }
}

