using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using DG.Tweening;

public class ElectricCar : MonoBehaviour
{
    //Settings
    public float patienceBubbleRatio = 0.5f;
    public float patienceLimit = 20f;
    public float defaultEnergyNeeded;
    public float energyNeeded;
    public float followSpeed = 5;
    public float energyPerSecond = 1f;
    public float moneyPerEnergy = 50f;
    // Connections
    public SplineFollower follower;
    public SplineComputer computer;
    public GameObject moneyPrefab;
    public DOTweenAnimation chargeTween;
    public GameObject waitBubble;

    ElectricPump electricPump;
    // State Variables
    bool isRecharging = false;
    bool needsEnergy = true;
    bool isFail = false;
    bool isTouchingCar = false;
    bool isTouchingPlayer = false;

    float moneyToPay;
    int spawnAmount;
    float patienceTimer = 0;
    // Start is called before the first frame update
    void Awake()
	{
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
        moneyToPay = moneyPerEnergy * energyNeeded;
        Move();
    }

    // Update is called once per frame
    void Update()
    {

        if (isRecharging && electricPump.currentAmountOfBatteries > 0 && needsEnergy)
        {
            
            electricPump.filler.fillAmount = Mathf.InverseLerp(defaultEnergyNeeded, 0, energyNeeded);
            energyNeeded -= energyPerSecond * Time.deltaTime;
            chargeTween.DOPlay();
         
        }
        else if(isRecharging &&  needsEnergy)
        {
            patienceTimer += Time.deltaTime;

            if (patienceTimer >= patienceLimit * patienceBubbleRatio && !waitBubble.activeInHierarchy)
            {
                waitBubble.SetActive(true);
            }


            if (patienceTimer >= patienceLimit)
            {
                energyNeeded = 0;
                patienceTimer = 0;
                chargeTween.DOPause();
                electricPump.ResetPump(true);
                needsEnergy = false;
            }
        }


        if (energyNeeded <= 0)
        {
            energyNeeded = 0;
            if (needsEnergy)
            {
                CalculateMoney();
                chargeTween.DOPause();
                electricPump.ResetPump(false);
                needsEnergy = false;
            }

            isRecharging = false;

            if (CheckIfCanMove())
            {
                Move();
                waitBubble.SetActive(false);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PumpStop") && needsEnergy)
        {
            PumpStop(other.gameObject);
        }

        if (other.CompareTag("ElectricCar"))
        {
            CarEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ElectricCar"))
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
        electricPump.SetMoney(spawnAmount, transform);
    }

    void OnEndReached(double startPercent)
    {
        patienceTimer = 0;
        energyNeeded = defaultEnergyNeeded; 
        needsEnergy = true;
        gameObject.SetActive(false);
    }

    void PumpStop(GameObject pump)
    {
        if (needsEnergy)
        {
            electricPump = pump.transform.parent.GetComponent<ElectricPump>();
            Stop();
            isRecharging = true;
        }
    }

    bool CheckIfCanMove()
    {
        return !(isTouchingCar || isTouchingPlayer || isRecharging);
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

