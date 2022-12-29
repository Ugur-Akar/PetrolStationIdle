using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System;

public class LevelManager : MonoBehaviour
{
    //Settings
    public int pumpCount = 4;
    public int startingMoney = 400;
    public StartingMoney startingMoneyParent;
    // Connections
    [Header("Parents")]
    public Transform lockParent;
    public Transform sourcePumpsParent;
    public Transform mainSourcesParent;

    [Header("Splines")]
    public List<SplineComputer> petrolSplines;
    public List<SplineComputer> electricSplines;
    [Header("Locks")]
    public List<Unlock> locks;

    [Header("Unlockables")]
    public List<GameObject> mainSources;
    public List<GameObject> sourcePumps;
    public List<GameObject> oilPumps;
    public List<GameObject> electricPumps;

    
    [Header("Tasks")]
    public List<TaskManager> taskManagerList;

    public event Action TaskDone;
    public event Action AllowUpgrades;

    [Header("UnlockableScripts")]
    public List<MainSource> mainSourceScripts;
    public List<SourcePump> sourcePumpScripts;
    public List<OilPump> oilPumpScripts;
    public List<ElectricPump> electricPumpScripts;

    // State Variables
    [Header("Indexes")]
    int lockIndex = 0;
    int petrolSplineIndex = -1;
    int electricSplineIndex = -1;
    int mainSourceIndex = -1;
    int sourcePumpIndex = -1;
    int oilSourceIndex = -1;
    int electricSourceIndex = -1;

    int oilSourceCounter = 0;
    int electricSourceCounter = 0;

    bool queueControl = false;

    //Static Vars
    public static int[] OIL_DEFAULT_PUMP_MAX_STACKS;
    public static int[] ELE_DEFAULT_PUMP_MAX_STACKS;

    // Start is called before the first frame update
    void Awake()
	{
        OIL_DEFAULT_PUMP_MAX_STACKS = new int[4];
        for (int i = 0; i < OIL_DEFAULT_PUMP_MAX_STACKS.Length; i++)
        {
            OIL_DEFAULT_PUMP_MAX_STACKS[i] = 1;
        }

        ELE_DEFAULT_PUMP_MAX_STACKS = new int[4];
        for(int i = 0; i < ELE_DEFAULT_PUMP_MAX_STACKS.Length; i++)
        {
            ELE_DEFAULT_PUMP_MAX_STACKS[i] = 1;
        }
		
        InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections()
    {

        if (queueControl)
        {
            SetUpLocks();
        }
        else
        {
            queueControl = true;
        }

        
    }

    void SetUpLocks()
    {
        for(int i = 0; i < lockParent.childCount; i++)
        {
            locks.Add(lockParent.GetChild(i).GetComponent<Unlock>());
        }

        for (int i = 0; i < locks.Count; i++)
        {
            locks[i].UnlockNextLock += UnlockNextLock;
            if (i > lockIndex)
            {
                locks[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < taskManagerList.Count; i++)
        {
            taskManagerList[i].TaskDone += TaskDoneCaller;
            if (i == PlayerPrefs.GetInt("taskIndex", 0))
            {
                taskManagerList[i].isCurrentTask = true;
            }
            else
            {
                taskManagerList[i].isCurrentTask = false;
            }
        }

        
    }
    void InitState(){
        
        if (PlayerPrefs.GetInt("taskIndex", 0) == 0)
        {
            startingMoneyParent.amountOfMoneyToActivate = startingMoney / PlayerManager.moneyValue;
            startingMoneyParent.ActivateMoney();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(oilSourceIndex > -1 && electricSourceIndex > -1 && !ES3.KeyExists("UpgradeAllowed"))
        {
            ES3.Save<bool>("UpgradeAllowed", true);
            AllowUpgrades();
        }
    }

    void UnlockNextLock()
    {
        lockIndex++;
        ES3.Save(nameof(lockIndex), lockIndex);

        if(lockIndex < locks.Count && lockIndex != 0)
        {
            locks[lockIndex].gameObject.SetActive(true);
        }
    }


    void TaskDoneCaller()
    {
        int taskIndex = PlayerPrefs.GetInt(nameof(taskIndex), 0);
        
        taskIndex++;

        if(taskIndex < taskManagerList.Count)
        {
            taskManagerList[taskIndex].isCurrentTask = true;
        }

        TaskDone();
    }

    void MainSourceUnlock(bool isOil)
    {
        if (isOil)
        {
            IncreaseOilSourceIndex();
        }
        else
        {
            IncreaseElectricSourceIndex();
        }

        for(int i = 0; i < sourcePumpScripts.Count; i++)
        {
            sourcePumpScripts[i].NewSourceUnlocked(isOil);
        }
    }

    void SourcePumpUnlock(GameObject newPump = null)
    {
        int index = sourcePumps.IndexOf(newPump);

        sourcePumpScripts[index].SetCollectibleDelay();

        for (int i = 0; i <= electricSourceIndex; i++)
        {
            sourcePumpScripts[index].NewSourceUnlocked(false);
        }

        for (int i = 0; i <= oilSourceIndex; i++)
        {
            sourcePumpScripts[index].NewSourceUnlocked(true);
        }
        
    }

    #region Activators
    public void LoadParameters()
    {
        if (ES3.KeyExists(nameof(petrolSplineIndex)))
        {
            petrolSplineIndex = ES3.Load<int>(nameof(petrolSplineIndex));
        }
        if (ES3.KeyExists(nameof(electricSplineIndex)))
        {
            electricSplineIndex = ES3.Load<int>(nameof(electricSplineIndex));
        }
        if (ES3.KeyExists(nameof(mainSourceIndex)))
        {
            mainSourceIndex = ES3.Load<int>(nameof(mainSourceIndex));
        }
        if (ES3.KeyExists(nameof(sourcePumpIndex)))
        {
            sourcePumpIndex = ES3.Load<int>(nameof(sourcePumpIndex));
        }
        if (ES3.KeyExists(nameof(oilSourceIndex)))
        {
            oilSourceIndex = ES3.Load<int>(nameof(oilSourceIndex));
        }
        if (ES3.KeyExists(nameof(electricSourceIndex)))
        {
            electricSourceIndex = ES3.Load<int>(nameof(electricSourceIndex));
        }

        if (ES3.KeyExists(nameof(lockIndex)))
        {
            lockIndex = ES3.Load<int>(nameof(lockIndex));
        }


        if (queueControl)
        {
            SetUpLocks();
        }

        queueControl = true;
    }

    public void ActivateUnlockedItems()
    {
        ActivateMainSources();
        ActivateSourcePumps();
        ActivatePumpsAndSplines();
        DeactivateUnlockedLocks();
    }

    void ActivateMainSources()
    {

        for(int i = 0; i < mainSourcesParent.childCount; i++)
        {
            mainSources.Add(mainSourcesParent.GetChild(i).gameObject);
            mainSourceScripts.Add(mainSources[i].GetComponent<MainSource>());

            if(mainSourceScripts[i].isOil && oilSourceCounter <= oilSourceIndex)
            {
                mainSources[i].SetActive(true);
                oilSourceCounter++;
            }
            else if(!mainSourceScripts[i].isOil && electricSourceCounter <= electricSourceIndex)
            {
                mainSources[i].SetActive(true);
                electricSourceCounter++;
            }
            
            mainSourceScripts[i].MainSourceUnlocked += MainSourceUnlock;
        }
        
    }

    void ActivateSourcePumps()
    {
        for (int i = 0; i < sourcePumpsParent.childCount; i++)
        {
            sourcePumps.Add(sourcePumpsParent.GetChild(i).gameObject);
            sourcePumpScripts.Add(sourcePumps[i].GetComponent<SourcePump>());
        }

        for (int i = 0; i < sourcePumpsParent.childCount; i++)
        {
            sourcePumpScripts[i].EnabledEvent += SourcePumpUnlock;
        }
        
        for (int i = 0; i <= sourcePumpIndex; i++)
        {
            sourcePumps[i].SetActive(true);
        }

    }

    void ActivatePumpsAndSplines()
    {

        for(int i = 0; i < petrolSplines.Count; i++)
        {
            if(i <= petrolSplineIndex)
            {
                petrolSplines[i].gameObject.SetActive(true);
                oilPumps[i].SetActive(true);
            }

            oilPumpScripts.Add(oilPumps[i].GetComponent<OilPump>());
        }

        for(int i = 0; i < electricSplines.Count; i++)
        {
            if (i <= electricSplineIndex)
            {
                electricSplines[i].gameObject.SetActive(true);
                electricPumps[i].SetActive(true);
            }

            electricPumpScripts.Add(electricPumps[i].GetComponent<ElectricPump>());
        }
    }

    void DeactivateUnlockedLocks()
    {
        for(int i = 0; i < lockIndex; i++)
        {
            locks[i].gameObject.SetActive(false);
        }

        if(lockIndex < locks.Count)
        {
            locks[lockIndex].unlockMoney = ES3.Load("UnlockMoney", locks[lockIndex].unlockMoney);
        }
    }

    #endregion

    public void OnOilPumpUpgradeButtonPressed(int indexOfPump)
    {
        oilPumpScripts[indexOfPump].IncreaseCapacity();
        Debug.Log("OilPumpIndex: " + indexOfPump);
    }

    public void OnElectricPumpUpgradeButtonPressed(int indexOfPump) 
    {
        electricPumpScripts[indexOfPump].IncreaseCapacity();
        Debug.Log("ElectricPumpIndex: " + indexOfPump);
    }


    public void OnOilSpeedBuyButtonPressed()
    {
        for(int i = 0; i < mainSources.Count; i++)
        {
            if(!mainSources[i].activeInHierarchy && mainSourceScripts[i].isOil)
            {
                mainSources[i].SetActive(true);
                break;
            }
        }
    }

    public void OnElectricSpeedBuyButtonPressed()
    {
        for (int i = 0; i < mainSources.Count; i++)
        {
            if (!mainSources[i].activeInHierarchy && !mainSourceScripts[i].isOil)
            {
                mainSources[i].SetActive(true);
                break;
            }
        }
    }

    public void OnOilStackBuyButtonPressed()
    {
        for(int i = 0; i < sourcePumps.Count; i++)
        {
            if(sourcePumpScripts[i].collectible.TryGetComponent<BarrelTweener>(out BarrelTweener bt))
            {
                sourcePumpScripts[i].OnStackLimitIncreased();
            }
        }
    }

    public void OnElectricStackBuyButtonPressed()
    {
        for (int i = 0; i < sourcePumps.Count; i++)
        {
            if (sourcePumpScripts[i].collectible.TryGetComponent<BatteryTweener>(out BatteryTweener bt))
            {
                sourcePumpScripts[i].OnStackLimitIncreased();
            }
        }
    }

    #region GetSet
    public int GetPetrolSplineIndex()
    {
        return petrolSplineIndex;
    }

    public int GetElectricSplineIndex()
    {
        return electricSplineIndex;
    }

    public int GetMainSourceIndex()
    {
        return mainSourceIndex;
    }

    public int GetSourcePumpIndex()
    {
        return sourcePumpIndex;
    }

    public void IncreaseMainSourceIndex()
    {
        mainSourceIndex++;
        ES3.Save(nameof(mainSourceIndex), mainSourceIndex);
    }

    public void IncreaseSourcePumpIndex()
    {
        sourcePumpIndex++;
        ES3.Save(nameof(sourcePumpIndex), sourcePumpIndex);
    }

    public void IncreasePetrolSplineIndex()
    {
        petrolSplineIndex++;
        ES3.Save(nameof(petrolSplineIndex), petrolSplineIndex);
    }

    public void IncreaseElectricSplineIndex()
    {
        electricSplineIndex++;
        ES3.Save(nameof(electricSplineIndex), electricSplineIndex);
    }

    public void IncreaseOilSourceIndex()
    {
        oilSourceIndex++;
        ES3.Save(nameof(oilSourceIndex), oilSourceIndex);
    }

    public void IncreaseElectricSourceIndex()
    {
        electricSourceIndex++;
        ES3.Save(nameof(electricSourceIndex), electricSourceIndex);
    }

    public int GetOilSourceIndex()
    {
        return oilSourceIndex;
    }
    public int GetElectricSourceIndex()
    {
        return electricSourceIndex;
    }

    public int GetOilRowNumber()
    {
        int oilRowNumber = 0;
        for(int i = 0; i < sourcePumpScripts.Count; i++)
        {
            if(sourcePumpScripts[i].collectible.TryGetComponent<BarrelTweener>(out BarrelTweener bt))
            {
                oilRowNumber = sourcePumpScripts[i].numberOfRows;
                break;
            }
        }
        
        return oilRowNumber;
    }

    public int GetElectricRowNumber()
    {
        int electricRowNumber = 0;
        for(int i = 0; i < sourcePumpScripts.Count; i++)
        {
            if(sourcePumpScripts[i].collectible.TryGetComponent<BatteryTweener>(out BatteryTweener bt))
            {
                electricRowNumber = sourcePumpScripts[i].numberOfRows;
                break;
            }
        }
        
        return electricRowNumber;
    }
    #endregion
}

