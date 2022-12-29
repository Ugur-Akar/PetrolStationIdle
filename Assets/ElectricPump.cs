using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;


public class ElectricPump : MonoBehaviour
{
    public bool isInactive = false;
    public bool isTask = false;
    [Header("Charge")]
    public bool canCharge = true;
    public static int absMaxLimit = 5;
    public int maxAmountOfBatteries = 1;
    public int currentAmountOfBatteries = 0;
    [Header("Tween")]
    public Transform tweenTarget;
    public DOTweenAnimation[] chargeTweens;
    public Transform[] tweeners;
    [Header("UI")]
    public TextMeshPro batteryText;
    public Image filler;
    //Settings

    // Connections
    public ObjectPool objectPool;
    TaskManager taskManager;
    // State Variables
    bool tweenIsPlaying = false;

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
        objectPool.Set += SetInactive;

        if (isTask)
        {
            taskManager = GetComponent<TaskManager>();
        }
    }
    void InitState()
    {
        int[] capacities = ES3.Load("ElectricPumpStacks", LevelManager.ELE_DEFAULT_PUMP_MAX_STACKS);
        maxAmountOfBatteries = capacities[transform.GetSiblingIndex()];

        SetUpUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (currentAmountOfBatteries >= maxAmountOfBatteries)
        {
            canCharge = false;
        }
        else
        {
            canCharge = true;
        }

        if (!tweenIsPlaying)
        {
            for (int i = 0; i < tweeners.Length; i++)
            {
                if (tweeners[i].localScale != Vector3.one)
                {
                    tweeners[i].localScale = Vector3.Lerp(tweeners[i].localScale, Vector3.one, Time.deltaTime * 5);
                }
            }
        }

        ChangeUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canCharge && !tweenIsPlaying)
            {
                foreach (DOTweenAnimation chargeTween in chargeTweens)
                    chargeTween.DORestart();
                tweenIsPlaying = true;
            }
            else if (!canCharge)
            {
                //We can add stuff to do if pump is full and player is trying to refill
            }

            if (isTask)
            {
                if (taskManager.isCurrentTask)
                {
                    taskManager.TaskCompletedAndEnabled();
                    isTask = false;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (canCharge && !tweenIsPlaying)
            {
                foreach (DOTweenAnimation chargeTween in chargeTweens)
                    chargeTween.DORestart();
                tweenIsPlaying = true;
            }
            else if (!canCharge && tweenIsPlaying)
            {
                foreach (DOTweenAnimation chargeTween in chargeTweens)
                {
                    chargeTween.DOPause();
                    chargeTween.DOSmoothRewind();
                }
                tweenIsPlaying = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (tweenIsPlaying)
            {
                foreach (DOTweenAnimation chargeTween in chargeTweens)
                    chargeTween.DOPause();
                tweenIsPlaying = false;
            }
        }
    }

    public void AddEnergy(GameObject battery)
    {
        Destroy(battery);
        currentAmountOfBatteries++;

        if(currentAmountOfBatteries >= maxAmountOfBatteries)
        {
            canCharge = false;
        }
        else
        {
            canCharge = true;
        }
    }

    public void ResetPump(bool isFail)
    {
        if(!isFail)
            currentAmountOfBatteries--;
        filler.fillAmount = 0;
    }

    public void SetMoney(int incomingAmount, Transform car)
    {
        objectPool.PayMoney(incomingAmount, car);
    }

    void SetInactive()
    {
        if (isInactive)
            gameObject.SetActive(false);
    }

    #region OwnUIRelated

    void SetUpUI()
    {
        currentAmountOfBatteries = 0;
        ChangeUI();
    }

    void ChangeUI()
    {
        batteryText.text = currentAmountOfBatteries + "/" + maxAmountOfBatteries;
    }
    #endregion

    public void IncreaseCapacity()
    {
        maxAmountOfBatteries++;

        int[] capacities = ES3.Load("ElectricPumpStacks", LevelManager.ELE_DEFAULT_PUMP_MAX_STACKS);
        capacities[transform.GetSiblingIndex()] = maxAmountOfBatteries;
        ES3.Save("ElectricPumpStacks", capacities);
    }

}

