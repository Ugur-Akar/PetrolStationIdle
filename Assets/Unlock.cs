using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class Unlock : MonoBehaviour
{
    int mc = 0;
    //Settings
    public bool isTask = false;
    public int unlockMoney = 50;
    public float moneyChangePerSecond = 25;
    public Vector3 indicatorDefaultScale;
    // Connections
    public GameObject[] unlockTweens;
    public List<GameObject> unlockTargets;
    public TextMeshPro moneyText;
    public List<DOTweenAnimation> indicatorTweens;

    public event Action<GameObject> Unlocked;
    public event Action UnlockNextLock;

    TaskManager taskManager;
    PlayerCollisionControl pcc;
    // State Variables
    public bool canUnlock = true;

    float currentMoney = 0;
    int shownMoney = 0;
    bool canChangeUIMoney = false;
    bool canLerp = false;

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
        if (isTask)
        {
            taskManager = GetComponent<TaskManager>();
        }
    }
    void InitState()
    {
        moneyText.text = "" + unlockMoney;
        currentMoney = unlockMoney;
        shownMoney = unlockMoney;
    }

    // Update is called once per frame
    void Update()
    {
        if(unlockMoney > 0)
        {
            canUnlock = true;
        }
        else
        {
            if (canUnlock)
            {
                PlayTween();
            }
            canUnlock = false;       
        }

        ChangeMoneyUI();

        if (canLerp)
        {
            for(int i = 0; i < indicatorTweens.Count; i++)
            {
                indicatorTweens[i].transform.localScale = Vector3.Lerp(transform.localScale, indicatorDefaultScale, Time.deltaTime);
            }
        }
    }

    #region Tweens
    public void PlayTween(int index = 0)
    {
        if (index < unlockTweens.Length)
        {
            unlockTweens[index].SetActive(true);
        }

        if (index >= unlockTweens.Length)
        {
            UnlockableTweens();
        }
    }

    public void UnlockableTweens()
    {
        for (int i = 0; i < unlockTargets.Count; i++)
        {
            unlockTargets[i].SetActive(true);
            unlockTargets[i].GetComponent<Unlockable>().PlayTweens();
            Unlocked(unlockTargets[i]);
        }

        UnlockNextLock();
        if(pcc != null)
            pcc.CallStopUnlock();

        if (taskManager != null)
        {
            taskManager.TaskCompletedAndDisable();
        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    
    #endregion

    void ChangeMoneyUI()
    {
        if (canChangeUIMoney)
        {
            if(unlockMoney != currentMoney)
            {
                currentMoney = Mathf.Lerp(currentMoney, unlockMoney, Time.deltaTime * moneyChangePerSecond);
                shownMoney = (int)currentMoney;
                moneyText.text = "" + shownMoney;
            }
        }
    }

    public bool MoneyArrived()
    {
        unlockMoney -= PlayerManager.moneyValue;
        canChangeUIMoney = true;
        if(unlockMoney <= 0)
        {
            ES3.DeleteKey("UnlockMoney");
        }

        if (unlockMoney < 0)
        {
            return true;
        }
        else
        {
            ES3.Save("UnlockMoney", unlockMoney);
            return false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {  

        if (other.CompareTag("Player"))
        {
            pcc = other.GetComponent<PlayerCollisionControl>();
            canLerp = false;

            for (int i = 0; i < indicatorTweens.Count; i++)
            {
                indicatorTweens[i].DORestart();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            for(int i = 0; i < indicatorTweens.Count; i++)
            {
                indicatorTweens[i].DOPause();
            }

            canLerp = true;
        }
    }

}

