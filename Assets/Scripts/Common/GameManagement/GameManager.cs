using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Dreamteck.Splines;
using GameAnalyticsSDK;

public class GameManager : MonoBehaviour
{
    // Settings
    public bool spawnLevel = true;
    public int firstLaunch = 1;
    public int tutorialLevels;

    // Connections
    public GameObject[] levels;
    public UIManager ui;
    public CarSpawnManager carSpawnManager;
    public PlayerManager playerManager;
    public UnlockManager unlockManager;
    public UpgradeCosts upgradeCosts;

    LevelManager levelManager;
    // State variables
    int currentLevel;
    int score;
 

    #region Initialization
    private void Awake()
    {

        InitStates();
        InitConnections();
    }


    void InitStates()
    {
        firstLaunch = PlayerPrefs.GetInt("FirstLaunch", 1);
        currentLevel = PlayerPrefs.GetInt("Level", 0); 


        LoadLevel();
        ActivateUnlockedInLevel();

        if (ES3.KeyExists("UpgradeAllowed"))
        {
            ui.AllowUpgrades();
        }
    }

    #region LevelInit
    void LoadLevel()
    {
        if (spawnLevel)
        {
            int prefabIndex = GetPrefabIndex(currentLevel, tutorialLevels, levels.Length);
            GameObject levelGO = Instantiate(levels[prefabIndex], Vector3.zero, Quaternion.identity);
            levelManager = levelGO.GetComponent<LevelManager>();
            levelManager.LoadParameters();
        }
    }

    void ActivateUnlockedInLevel()
    {
        levelManager.ActivateUnlockedItems();
    }

    #endregion
    int GetPrefabIndex(int levelIndex, int nInitialLevels, int nLevels)
    {

        int nRepeatingLevels = nLevels - nInitialLevels;
        int prefabIndex = levelIndex;
        if (levelIndex >= nInitialLevels)
        {
            prefabIndex = ((levelIndex - nInitialLevels) % nRepeatingLevels) + nInitialLevels;
        }
        return prefabIndex;

    }

    void InitConnections()
    {
        EventConfiguration();

        playerManager.arrowManager.SetArrowManager(levelManager.taskManagerList);
        
        carSpawnManager.SetUpCarSpawner(levelManager.petrolSplines, levelManager.electricSplines, levelManager.GetPetrolSplineIndex(), levelManager.GetElectricSplineIndex());
        unlockManager.SetLocks(levelManager.locks);
    }

    void EventConfiguration()
    {
        ui.OnLevelStart += OnLevelStart;
        ui.OnNextLevel += OnNextLevel;
        ui.OnLevelRestart += OnLevelRestart;
        ui.OilSpeedBuyButtonPressed += OnOilSpeedBuyButtonPressed;
        ui.OilStackBuyButtonPressed += OnOilStackBuyButtonPressed;
        ui.ElectricStackBuyButtonPressed += OnElectricStackBuyButtonPressed;
        ui.ElectricSpeedBuyButtonPressed += OnElectricSpeedBuyButtonPressed;
        ui.OilPumpUpgradeButtonPressed += OnOilPumpUpgradeButtonPressed;
        ui.ElectricPumpUpgradeButtonPressed += OnElectricPumpUpgradeButtonPressed;

        unlockManager.UnlockedEvent += Unlocked;

        playerManager.MoneyChangedEvent += MoneyChanged;
        playerManager.ShakeMoneyUI += ShakeMoneyUIStart;
        playerManager.PunchMoneyUI += PunchMoneyUI;

        levelManager.TaskDone += TaskDone;
        levelManager.AllowUpgrades += AllowUpgrades;
    }

    #endregion
    private void Start()
    {
        GameAnalytics.Initialize();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < 50; i++)
            {
                Debug.Log("Prefab index for level " + i + ":" + GetPrefabIndex(i, tutorialLevels, levels.Length));
            }
        }
    }

    #region LevelEndRelated
    void OnLevelFailed()
    {
        ui.FailLevel();
        Debug.Log("LEVEL FAILED");
        
    }

    void OnFinishLevel()
    {
        ui.FinishLevel();
        PlayerPrefs.SetInt("Level", currentLevel + 1);
    }

    void OnLevelStart()
    {
        Debug.Log("LEVEL STARTED");
        UpdateUpgradeUI();
        carSpawnManager.canSpawn = true;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "GameStart");
    }

    void OnLevelRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PlayerPrefs.SetInt("showStart", 0);
    }

    #endregion

    #region GameManagerRelated

    void Unlocked(GameObject go)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "UnlockedNewObject");
        if (go.TryGetComponent<PetrolCarPool>(out PetrolCarPool pcp))
        {
            carSpawnManager.Unlocked(go);
            levelManager.IncreasePetrolSplineIndex();
            Debug.Log("Oil");
        }
        else if (go.TryGetComponent<ElectricCarPool>(out ElectricCarPool ecp))
        {
            carSpawnManager.Unlocked(go);
            levelManager.IncreaseElectricSplineIndex();
        }
        else if (go.TryGetComponent<SourcePump>(out SourcePump sp))
        {
            levelManager.IncreaseSourcePumpIndex();
        }
        else if (go.TryGetComponent<MainSource>(out MainSource ms))
        {
            levelManager.IncreaseMainSourceIndex();
        }
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "UnlockedNewObject");
    }

    #endregion

    #region PlayerManagerRelated

    void MoneyChanged(bool isInstant = false)
    {
        ES3.Save(nameof(playerManager.money), playerManager.money);
        ui.SetTargetScoreUI(playerManager.money, isInstant);
    }

    void ShakeMoneyUIStart()
    {
        ui.ShakeMoneyUIStart();
    }
    
    void PunchMoneyUI()
    {
        ui.PunchMoneyUI();
    }

    #endregion

    #region LevelManagerRelated

    void TaskDone()
    {
        playerManager.arrowManager.ChangeTarget();
    }
    //Menu1

    void OnOilPumpUpgradeButtonPressed(int pumpIndex)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "OilPumpUpgrade");
        if(levelManager.oilPumpScripts[pumpIndex].maxAmountOfBarrels < OilPump.absMaxLimit && 
            playerManager.money >= upgradeCosts.pumpStackCosts[levelManager.oilPumpScripts[pumpIndex].maxAmountOfBarrels - 1])
        {
            playerManager.money -= upgradeCosts.pumpStackCosts[levelManager.oilPumpScripts[pumpIndex].maxAmountOfBarrels - 1];
            levelManager.OnOilPumpUpgradeButtonPressed(pumpIndex);
            UpdateUpgradeUI();
            MoneyChanged(true);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "OilPumpUpgrade");            
        }
        else
        {
            if(levelManager.oilPumpScripts[pumpIndex].maxAmountOfBarrels < OilPump.absMaxLimit)
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Limit, "OilPumpUpgrade");
            }
            else
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "OilPumpUpgrade");
            }
        }

    }

    void OnElectricPumpUpgradeButtonPressed(int pumpIndex)
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "ElectricPumpUpgrade");
        if (levelManager.electricPumpScripts[pumpIndex].maxAmountOfBatteries < ElectricPump.absMaxLimit &&
            playerManager.money >= upgradeCosts.pumpStackCosts[levelManager.electricPumpScripts[pumpIndex].maxAmountOfBatteries - 1])
        {
            playerManager.money -= upgradeCosts.pumpStackCosts[levelManager.electricPumpScripts[pumpIndex].maxAmountOfBatteries - 1];
            levelManager.OnElectricPumpUpgradeButtonPressed(pumpIndex);
            UpdateUpgradeUI();
            MoneyChanged(true);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "ElectricPumpUpgrade");
        }
        else
        {
            if(levelManager.electricPumpScripts[pumpIndex].maxAmountOfBatteries < ElectricPump.absMaxLimit)
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Limit, "ElectricPumpUpgrade");
            }
            else
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "ElectricPumpUpgrade");
            }
        }
    }



    //Menu2
    void OnOilSpeedBuyButtonPressed()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "PumpjackBuy");
        if (levelManager.GetOilSourceIndex() + 1 < upgradeCosts.sourceSpeedCosts.Length)
        {
            if (playerManager.money >= upgradeCosts.sourceSpeedCosts[levelManager.GetOilSourceIndex() + 1])
            {
                playerManager.money -= upgradeCosts.sourceSpeedCosts[levelManager.GetOilSourceIndex() + 1];
                levelManager.OnOilSpeedBuyButtonPressed();
                UpdateUpgradeUI();
                MoneyChanged(true);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "PumpjackBuy");
            }
            else
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "PumpjackBuy");
            }
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Limit, "PumpjackBuy");
        }
    }

    void OnElectricSpeedBuyButtonPressed()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "TutbineBuy");
        if (levelManager.GetElectricSourceIndex() + 1 < upgradeCosts.sourceSpeedCosts.Length)
        {
            if (playerManager.money >= upgradeCosts.sourceSpeedCosts[levelManager.GetElectricSourceIndex() + 1])
            {
                playerManager.money -= upgradeCosts.sourceStackCosts[levelManager.GetElectricSourceIndex() + 1];
                levelManager.OnElectricSpeedBuyButtonPressed();
                UpdateUpgradeUI();
                MoneyChanged(true);
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "TutbineBuy");
            }
            else
            {
                GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "TutbineBuy");
            }
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Limit, "TutbineBuy");
        }
    }


    void OnOilStackBuyButtonPressed()
    {
        int uCost = 0;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "OilStackBuy");
        for (int i = 0; i < levelManager.sourcePumpScripts.Count; i++)
        {
            if(levelManager.sourcePumpScripts[i].collectible.TryGetComponent<BarrelTweener>(out BarrelTweener bt))
            {
                uCost = upgradeCosts.sourceStackCosts[levelManager.sourcePumpScripts[i].numberOfRows - 1];
                break;
            }           
        }

        if (playerManager.money >= uCost)
        {
            levelManager.OnOilStackBuyButtonPressed();
            playerManager.money -= uCost;
            UpdateUpgradeUI();
            MoneyChanged(true);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "OilStackBuy");
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "OilStackBuy");
        }
    }

    void OnElectricStackBuyButtonPressed()
    {
        int uCost = 0;
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, "ElectricStackBuy");
        for (int i = 0; i < levelManager.sourcePumpScripts.Count; i++)
        {
            if (levelManager.sourcePumpScripts[i].collectible.TryGetComponent<BatteryTweener>(out BatteryTweener bt))
            {
                uCost = upgradeCosts.sourceStackCosts[levelManager.sourcePumpScripts[i].numberOfRows - 1];
                break;
            }
        }

        if (playerManager.money >= uCost)
        {
            levelManager.OnElectricStackBuyButtonPressed();
            playerManager.money -= uCost;
            UpdateUpgradeUI();
            MoneyChanged(true);
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "ElectricStackBuy");
        }
        else
        {
            GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, "ElectricStackBuy");
        }
    }

    
    void AllowUpgrades()
    {
        UpdateUpgradeUI();
        ui.AllowUpgrades();
    }

    void UpdateUpgradeUI()
    {
        //Oil
        int oilCostIndex = levelManager.GetOilSourceIndex() + 1;

        if (oilCostIndex < upgradeCosts.sourceSpeedCosts.Length)
        {
            ui.UpgradeMenu2_UpdateOilSpeed(upgradeCosts.sourceSpeedCosts[oilCostIndex], oilCostIndex);
        }
        else
        {
            ui.UpgradeMenu2_UpdateOilSpeed(upgradeCosts.sourceSpeedCosts[upgradeCosts.sourceSpeedCosts.Length - 1], levelManager.GetOilSourceIndex() + 1);
        }

        int oilRowNumber = levelManager.GetOilRowNumber();

        if(oilRowNumber <= SourcePump.maxNumberOfRows)
        {
            ui.UpgradeMenu2_UpdateOilStack(upgradeCosts.sourceStackCosts[oilRowNumber - 1], oilRowNumber);
        }
        else
        {
            ui.UpgradeMenu2_UpdateOilStack(upgradeCosts.sourceStackCosts[upgradeCosts.sourceStackCosts.Length - 1], SourcePump.maxNumberOfRows);
        }

        int[] oilUpgLevels = ES3.Load("OilPumpStacks", LevelManager.OIL_DEFAULT_PUMP_MAX_STACKS);
        int[] oilUpgCosts = new int[oilUpgLevels.Length];

        for(int i = 0; i < oilUpgLevels.Length; i++)
        {
            oilUpgCosts[i] = upgradeCosts.pumpStackCosts[oilUpgLevels[i] - 1];
        }

        ui.UpgradeMenu1_UpdateOilStack(oilUpgLevels, oilUpgCosts);


        //Electric
        int eleCostIndex = levelManager.GetElectricSourceIndex() + 1;

        if(eleCostIndex < upgradeCosts.sourceSpeedCosts.Length)
        {
            ui.UpgradeMenu2_UpdateElectricSpeed(upgradeCosts.sourceSpeedCosts[eleCostIndex], eleCostIndex);
        }
        else
        {
            ui.UpgradeMenu2_UpdateElectricSpeed(upgradeCosts.sourceSpeedCosts[upgradeCosts.sourceSpeedCosts.Length - 1], levelManager.GetElectricSourceIndex() + 1);
        }

        int eleRowNumber = levelManager.GetElectricRowNumber();

        if(eleRowNumber <= SourcePump.maxNumberOfRows)
        {
            ui.UpgradeMenu2_UpdateElectricStack(upgradeCosts.sourceStackCosts[eleRowNumber - 1], eleRowNumber);
        }
        else
        {
            ui.UpgradeMenu2_UpdateElectricStack(upgradeCosts.sourceStackCosts[upgradeCosts.sourceStackCosts.Length - 1], SourcePump.maxNumberOfRows);
        }

        int[] eleUpgLevels = ES3.Load("ElectricPumpStacks", LevelManager.ELE_DEFAULT_PUMP_MAX_STACKS);
        int[] eleUpgCosts = new int[eleUpgLevels.Length];

        for(int i = 0; i < eleUpgLevels.Length; i++)
        {
            eleUpgCosts[i] = upgradeCosts.pumpStackCosts[eleUpgLevels[i] - 1];
        }

        ui.UpgradeMenu1_UpdateElectricStack(eleUpgLevels, eleUpgCosts);

    }
    #endregion


    private void OnApplicationQuit()
    {
        GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, "GameStart");
    }
}