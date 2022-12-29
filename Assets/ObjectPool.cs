using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class ObjectPool : MonoBehaviour
{
    //Settings
    public int amountToPool;
    public int rowCount = 3;
    public int columnCount = 3;
    public float xShift = 0.3f;
    public float yShift = 0.2f;
    public float zShift = 0.5f;

    // Connections
    public GameObject objectToPool;
    public List<GameObject> pooledObjects;
    public List<Money> moneyList;
    public GameObject[] objects;
    public List<Vector3> positions;
    public Transform startPoint;

    public event Action Set;
    // State Variables

    // Start is called before the first frame update
    void Awake()
	{
        positions = new List<Vector3>();
        objects = new GameObject[amountToPool];
        moneyList = new List<Money>();
        pooledObjects = new List<GameObject>();

        Vector3 positionToAdd = startPoint.position;

        while (positions.Count < amountToPool)
        {

            for (int i = 0; i < columnCount; i++)
            {
                for (int j = 0; j < rowCount; j++)
                {
                    positions.Add(positionToAdd);
                    positionToAdd.z += zShift;
                }
                positionToAdd.z = startPoint.position.z;
                positionToAdd.x += xShift;
            }

            positionToAdd.x = startPoint.position.x;
            positionToAdd.y += yShift;
        }
        //InitConnections();

        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(objectToPool, transform);
            objects[i] = tmp;
            Money money = tmp.GetComponent<Money>();
            moneyList.Add(money);
            tmp.SetActive(false);
            pooledObjects.Add(tmp);
        }
    }
    void Start()
    {
        //InitState();

        

        //Set?.Invoke();
    }
    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }


    public void PayMoney(int amount, Transform car)
    {
        for(int i = 0; i < amount; i++)
        {
            GameObject newMoneyGo = GetPooledObject();
            if (newMoneyGo != null)
            {
                newMoneyGo.transform.position = car.position;
                newMoneyGo.SetActive(true);
                newMoneyGo.GetComponent<Money>().MoveToPosition(positions[pooledObjects.IndexOf(newMoneyGo)]);
            }
        }     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach(Money money in moneyList)
            {
                money.MoveToPlayer(other.transform);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Money money in moneyList)
            {
                money.MoveToPlayer(other.transform);
            }
        }
    }

    public float SendUnlockMoney(Vector3 targetPosition)
    {
        GameObject newMoneyGo = GetPooledObject();
        if (newMoneyGo != null)
        {
            newMoneyGo.transform.position = transform.position;
            newMoneyGo.SetActive(true);
            Money money = newMoneyGo.GetComponent<Money>();
            money.MoveToLock(targetPosition);
            return money.moveToLockDuration / 2;
        }
        Debug.Break();
        return 0;
    }

    public void ActivateStartingMoney(int amount)
    {
        Debug.Log("Starts:" + amount);
        for(int i = 0; i < amount; i++)
        {
            GameObject tmp = GetPooledObject();
            if(tmp != null)
            {
                tmp.transform.position = positions[i];
                tmp.SetActive(true);
            }
        }
    }
}

