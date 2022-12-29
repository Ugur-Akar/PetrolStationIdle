using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;


public class MainSourceUnlocker : MonoBehaviour
{
    //Settings
    public bool isTask = false;
    public int unlockMoney = 10;
    public float moneyChangePerSecond = 25;
    public Vector3 indicatorDefaultScale = Vector3.one;
    // Connections
    public GameObject[] unlockTweens;
    public List<GameObject> unlockTargets;
    public TextMeshPro moneyText;
    public List<DOTweenAnimation> indicatorTweens;

    public event Action<GameObject> msUnlocked;
    public event Action msUnlockNext;

    public event Action TaskDone;

    TaskManager taskManager;
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
    void InitConnections(){
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
        if (unlockMoney > 0)
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
            for (int i = 0; i < indicatorTweens.Count; i++)
            {
                indicatorTweens[i].transform.localScale = Vector3.Lerp(transform.localScale, indicatorDefaultScale, Time.deltaTime);
            }
        }
    }

    #region UnlockTweens

    public void PlayTween(int index = 0)
    {
        if(index < unlockTweens.Length)
        {
            unlockTweens[index].SetActive(true);
        }

        if(index >= unlockTweens.Length)
        {
            
        }
    }

    #endregion

    void ChangeMoneyUI()
    {
        if (canChangeUIMoney)
        {
            if (unlockMoney != currentMoney)
            {
                currentMoney = Mathf.Lerp(currentMoney, unlockMoney, Time.deltaTime * moneyChangePerSecond);
                shownMoney = (int)currentMoney;
                moneyText.text = "" + shownMoney;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Money"))
        {
            unlockMoney -= PlayerManager.moneyValue;
            canChangeUIMoney = true;
        }
        if (other.CompareTag("Player"))
        {
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
            for (int i = 0; i < indicatorTweens.Count; i++)
            {
                indicatorTweens[i].DOPause();
            }

            canLerp = true;
        }
    }
}

