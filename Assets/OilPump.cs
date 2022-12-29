using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class OilPump : MonoBehaviour
{
    public bool isInactive = false;
    public bool isTask = false;

    [Header("Refill")]
    public bool canRefill = true;
    public static int absMaxLimit = 5;
    public int maxAmountOfBarrels = 1;
    public int currentAmountOfBarrels = 0;
    [Header("Tween")]
    public Transform tweenTarget;
    public DOTweenAnimation[] refillTweens;
    public Transform[] tweeners;
    [Header("UI")]
    public TextMeshPro barrelText;
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
        int[] capacities = ES3.Load("OilPumpStacks", LevelManager.OIL_DEFAULT_PUMP_MAX_STACKS);
        maxAmountOfBarrels = capacities[transform.GetSiblingIndex()];

        SetUpUI();
    }

    // Update is called once per frame
    void Update()
    {
        if(currentAmountOfBarrels >= maxAmountOfBarrels)
        {
            canRefill = false;
        }
        else
        {
            canRefill = true;
        }

        if(!tweenIsPlaying)
        {
            for(int i = 0; i < tweeners.Length; i++)
            {
                if(tweeners[i].localScale != Vector3.one)
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
            if (canRefill && !tweenIsPlaying)
            {
                foreach(DOTweenAnimation refillTween in refillTweens)
                    refillTween.DORestart();

                tweenIsPlaying = true;
            }
            else if (!canRefill)
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
            if(canRefill && !tweenIsPlaying)
            {
                foreach (DOTweenAnimation refillTween in refillTweens)
                    refillTween.DORestart();

                tweenIsPlaying = true;
            }
            else if(!canRefill && tweenIsPlaying)
            {
                foreach (DOTweenAnimation refillTween in refillTweens)
                    refillTween.DOPause();

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
                foreach (DOTweenAnimation refillTween in refillTweens)
                    refillTween.DOPause();       
                
                tweenIsPlaying = false;
            }
        }
    }

    public void AddFuel(GameObject barrel)
    {
        currentAmountOfBarrels++;
        Destroy(barrel);

        if(currentAmountOfBarrels >= maxAmountOfBarrels)
        {
            canRefill = false;
        }
        else
        {
            canRefill = true;
        }
    }


    public void ResetPump(bool isFail)
    {
        if(!isFail)
            currentAmountOfBarrels--;
        filler.fillAmount = 0;
    }

    public void SetMoney(int incomingAmount, Transform car)
    {
        objectPool.PayMoney(incomingAmount, car);
    }

    void SetInactive()
    {
        if(isInactive)
            gameObject.SetActive(false);
    }

    #region OwnUIRelated

    void SetUpUI()
    {
        currentAmountOfBarrels = 0;
        ChangeUI();
    }

    void ChangeUI()
    {
        barrelText.text = currentAmountOfBarrels + "/" + maxAmountOfBarrels;
    }
    #endregion

    public void IncreaseCapacity()
    {
        maxAmountOfBarrels++;

        int[] capacities = ES3.Load("OilPumpStacks", LevelManager.OIL_DEFAULT_PUMP_MAX_STACKS);
        capacities[transform.GetSiblingIndex()] = maxAmountOfBarrels;
        ES3.Save("OilPumpStacks", capacities);
    }

}

