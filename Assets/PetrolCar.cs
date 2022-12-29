using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;


public class PetrolCar : MonoBehaviour
{
    //Settings
    public float patienceBubbleRatio = 0.5f;
    public float patienceLimit = 20f;
    public float defaultOilNeeded;
    public float oilNeeded;
    public float followSpeed = 5;
    public float oilPerSecond = 1f;
    public float moneyPerFuel = 200f;
    // Connections
    public SplineFollower follower;
    public SplineComputer computer;
    public GameObject moneyPrefab;
    public DOTweenAnimation fuelTween;
    public GameObject waitBubble;

    OilPump oilPump;
    // State Variables
    bool isRefilling = false;
    bool needsOil = true;
    bool isFail = false;
    public bool isTouchingPlayer = false;
    public bool isTouchingCar = false;

    float moneyToPay;
    int spawnAmount;
    float patienceTimer = 0;
    // Start is called before the first frame update
    void Awake()
	{
        follower = GetComponent<SplineFollower>();
		InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections()
    {        
        follower.onEndReached += OnEndReached;
    }
    void InitState()
    {
        moneyToPay = moneyPerFuel * oilNeeded;
        Move();
    }

    // Update is called once per frame
    void Update()
    {
        if (isRefilling && oilPump.currentAmountOfBarrels > 0 && needsOil)
        {
            oilPump.filler.fillAmount = Mathf.InverseLerp(defaultOilNeeded, 0, oilNeeded);
            oilNeeded -= oilPerSecond * Time.deltaTime;
            fuelTween.DOPlay();           
        }
        else if(isRefilling && needsOil)
        {
            patienceTimer += Time.deltaTime;

            if (patienceTimer >= patienceLimit * patienceBubbleRatio && !waitBubble.activeInHierarchy)
            {
                waitBubble.SetActive(true);
            }

            if (patienceTimer >= patienceLimit)
            {
                oilNeeded = 0;
                patienceTimer = 0;
                fuelTween.DOPause();
                oilPump.ResetPump(true);
                needsOil = false;
            }
        }

        if(oilNeeded <= 0)
        {
            oilNeeded = 0;
            if (needsOil)
            {
                CalculateMoney();
                fuelTween.DOPause();
                oilPump.ResetPump(false);
                needsOil = false;
            }
            
            isRefilling = false;

            if (CheckIfCanMove())
            {
                Move();
                waitBubble.SetActive(false);
            }
        }

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PumpStop"))
        {
            PumpStop(other.gameObject);
        }

        if (other.CompareTag("PetrolCar"))
        {
            CarEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PetrolCar"))
        {
            CarExit();
        }

        if (other.CompareTag("PumpStop"))
        {

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
    public void Move()
    {
        follower.followSpeed = followSpeed;
    }

    public void Stop()
    {
        follower.followSpeed = 0;
    }

    void CalculateMoney()
    {
        spawnAmount = Mathf.FloorToInt(moneyToPay / PlayerManager.moneyValue);
        oilPump.SetMoney(spawnAmount, transform);
    }

    void OnEndReached(double startPercent)
    {
        patienceTimer = 0;
        oilNeeded = defaultOilNeeded;
        needsOil = true;
        gameObject.SetActive(false);
    }

    void PumpStop(GameObject pump)
    {
        if (needsOil)
        {
            oilPump = pump.transform.parent.GetComponent<OilPump>();
            Stop();
            isRefilling = true;
        }
    }
    
    bool CheckIfCanMove()
    {
        return !(isTouchingCar || isTouchingPlayer || isRefilling);
    }

    void CarEnter()
    {
        isTouchingCar = true;
        Stop();
    }

    void CarExit()
    {
        isTouchingCar = false;

        if (CheckIfCanMove())
        {
            Move();
        }
    }

    void PlayerEnter()
    {
        isTouchingPlayer = true;
        Stop();
    }

    void PlayerExit()
    {
        isTouchingPlayer = false;
        
        if (CheckIfCanMove())
        {
            Move();
        }
    }
}

