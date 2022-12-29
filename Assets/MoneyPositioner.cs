using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoneyPositioner : MonoBehaviour
{
    //Settings
    public float xShift = 0.5f;
    public float yShift = 0.5f;
    public float zShift = 0.5f;

    public int rowCount = 3;
    public int columnCount = 3;
    public int maxAmountOfMoney = 500;
    // Connections
    public Transform startPoint;
    public GameObject moneyGO;

    Transform payingCar;
    // State Variables
    public int targetAmount = 0;
    int countPerFloor;
    int tweenTargetIndex;
    public int moneyAmount = 0;

    public List<Vector3> positions;
    public List<GameObject> stacks;
    List<Money> moneyList;
    // Start is called before the first frame update
	void Awake()
	{
		InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections(){
        
    }
    void InitState(){
        moneyList = new List<Money>();
        stacks = new List<GameObject>();
        positions = new List<Vector3>();
        CalculatePositions();
        CreateMoney();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CalculatePositions()
    {
        Vector3 positionToAdd = startPoint.position;
        
        while(positions.Count < maxAmountOfMoney)
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
    }

    void CreateMoney()
    {
        for(int i = 0; i < maxAmountOfMoney; i++)
        {
            GameObject newMoneyGO = Instantiate(moneyGO, positions[i], Quaternion.identity, transform);
            Money money = newMoneyGO.GetComponent<Money>();
            stacks.Add(newMoneyGO);
            moneyList.Add(money);
            money.defaultPosition = positions[i];
        }

        foreach(GameObject money in stacks)
        {
            money.SetActive(false);
        }
    }

    public void IncomingMoney(int incomingAmount, Transform car)
    {
        payingCar = car;
        int moneyToActivate = incomingAmount;
        for(int i = 0; i < stacks.Count; i++)
        {
            if (!stacks[i].activeInHierarchy && moneyToActivate > 0)
            {
                moneyToActivate--;
                stacks[i].SetActive(true);
                stacks[i].transform.position = payingCar.position;
            }
        }
    }







    //public void IncomingMoney(int incomingAmount)
    //{
    //    targetAmount += incomingAmount;
    //    CalculatePositions();
    //}

    //public void CalculatePositions()
    //{
    //    Vector3 positionToAdd = startPoint.position;

    //    int height = targetAmount / countPerFloor;

    //    for(int h = 0; h <= height; h++)
    //    {
    //        for(int i = 0; i < columnCount; i++)
    //        {
    //            for(int j = 0; j < rowCount; j++)
    //            {
    //                positions.Add(positionToAdd);
    //                positionToAdd.z += zShift;
    //            }
    //            positionToAdd.z = startPoint.position.z;
    //            positionToAdd.x += xShift;
    //        }
    //        positionToAdd.x = startPoint.position.x;
    //        positionToAdd.y += yShift;
    //    }

    //    PlaceCurrentStacks();
    //}

    //public void PlaceCurrentStacks()
    //{
        
    //    for(tweenTargetIndex = 0; tweenTargetIndex < stacks.Count; tweenTargetIndex++)
    //    {
    //        stacks[tweenTargetIndex].transform.position = positions[tweenTargetIndex];            
    //    }
        
        

    //    if(stacks.Count != tweenTargetIndex)
    //    {
    //        Debug.Log("Not Equal");
    //        Debug.Break();
    //    }
        
    //}

    //public void AddMoney(Transform moneyTransform)
    //{
    //    moneyTransform.parent = transform;        
    //    Money money = moneyTransform.GetComponent<Money>();
    //    stacks.Add(money.gameObject);
    //    moneyList.Add(money);
    //    moneyTransform.GetChild(0).GetComponent<Collider>().enabled = true;
    //    PlaceNewMoney();
    //}

    //void PlaceNewMoney()
    //{
    //    moneyList[tweenTargetIndex].MoveToPosition(positions[tweenTargetIndex]);
    //    tweenTargetIndex++;
    //}


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //foreach(Money money in moneyList)
    //        //{
    //        //    money.MoveToPlayer(other.transform);
    //        //}
    //        //while(transform.childCount > 0)
    //        //{
    //        //    transform.GetChild(0).parent = null;
    //        //}
            
    //        while(moneyList.Count > 0)
    //        {
    //            moneyList[0].transform.parent = null;
    //            stacks.RemoveAt(0);
    //            moneyList[0].MoveToPlayer(other.transform);
    //            moneyList.RemoveAt(0);
    //        }

    //        targetAmount = 0;
    //        tweenTargetIndex = 0;
    //        positions.Clear();
    //    }
    //}

    

}

