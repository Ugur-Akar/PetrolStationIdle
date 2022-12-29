using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class UIManager : MonoBehaviour
{
    const float DEFAULT_START_DELAY = 0.2f;

    public  Action OnLevelStart, 
                    OnNextLevel, 
                    OnLevelRestart, 
                    OnGamePaused, 
                    OnGameResumed, 
                    OnInGameRestart;

    [Header("Settings")]
    public bool defaultPauseOperations = true;
    public float uiMoneyChangePerSecond = 10;
    public float lerpMargin = 0.01f;
    public float shakePower = 5;
    public int shakeVibrato = 20;
    public float shakeRandomness = 90;
    public Vector3 punchVector;
    public float punchDuration = 1;

    [Header("Screens")]
    public GameObject startCanvas;
    public GameObject ingameCanvas;
    public GameObject finishCanvas;
    public GameObject failCanvas;

    [Header("UpgradeMenu")]
    public GameObject pauseMenu;
    public GameObject upgradeMainMenu;
    public GameObject upgradeMenu_1;
    public GameObject upgradeMenu_2;
    public GameObject[] menuOpenButtons;

    [Header("UpgradeButtons")]
    public TextMeshProUGUI oilSpeedLevel;
    public TextMeshProUGUI oilStackLevel;
    public TextMeshProUGUI oilSpeedCost;
    public TextMeshProUGUI oilStackCost;

    public TextMeshProUGUI electricSpeedLevel;
    public TextMeshProUGUI electricStackLevel;
    public TextMeshProUGUI electricSpeedCost;
    public TextMeshProUGUI electricStackCost;

    public TextMeshProUGUI[] oilPumpLevels;
    public TextMeshProUGUI[] oilPumpCosts;
    public TextMeshProUGUI[] electricPumpLevels;
    public TextMeshProUGUI[] electricPumpCosts;

    public event Action<int> OilPumpUpgradeButtonPressed;
    public event Action<int> ElectricPumpUpgradeButtonPressed;


    public event Action OilSpeedBuyButtonPressed;
    public event Action OilStackBuyButtonPressed;

    public event Action ElectricSpeedBuyButtonPressed;
    public event Action ElectricStackBuyButtonPressed;

    [Header("In Game")]
    public LevelBarDisplay levelBarDisplay;
    public TextMeshProUGUI inGameScoreText;
    public GameObject moneyBox;

    Vector3 moneyBoxPosition;
    Vector3 moneyBoxScale;
    [Header("Finish Screen")]
    public ScoreTextManager scoreText;

    // State variables
    float timeScale;
    bool canUpdateMoney = false;
    float targetMoney = 0;
    float shownMoney = 0;

    bool punchMoney = true;
    bool shakeMoney = true;

    void Start()
    {
        InitState();   
    }
    
    void InitState()
    {
        timeScale = Time.timeScale;

        moneyBoxPosition = moneyBox.transform.position;
        moneyBoxScale = moneyBox.transform.localScale;
    }

    #region Handler Functions

    public void StartLevelButton()
    {
        OnLevelStart?.Invoke();
        
    }

    public void NextLevelButton()
    {
        PlayerPrefs.SetInt("displayStart", 0);
        OnNextLevel?.Invoke();

    }

    public void RestartLevelButton()
    {
        PlayerPrefs.SetInt("displayStart", 0);
        OnLevelRestart?.Invoke();
    }

    public void OnPauseButtonPressed()
    {
        pauseMenu.SetActive(true); 
        if (defaultPauseOperations)
        {
            //timeScale = Time.timeScale; // Restore the current time scale to use in Resume button
            Time.timeScale = 0;
        }

        for (int i = 0; i < menuOpenButtons.Length; i++)
        {
            menuOpenButtons[i].SetActive(false);
        }

        OnGamePaused?.Invoke();
    }

    public void OnResumeButtonPressed()
    {
        pauseMenu.SetActive(false);
        if (defaultPauseOperations)
        {
            Time.timeScale = timeScale;
        }

        menuOpenButtons[0].SetActive(true);
        if (ES3.KeyExists("UpgradeAllowed"))
        {
            menuOpenButtons[1].SetActive(true);
        }

        OnGameResumed?.Invoke();
    }

    public void OnInGameRestartPressed()
    {
        if (defaultPauseOperations)
        {
            Time.timeScale = timeScale;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        OnInGameRestart?.Invoke();
    }

    public void OnUpgradeMenuButtonPressed()
    {
        upgradeMainMenu.SetActive(true);

        if (defaultPauseOperations)
        {
            //timeScale = Time.timeScale; // Restore the current time scale to use in Resume button
            Time.timeScale = 0;
        }

        for (int i = 0; i < menuOpenButtons.Length; i++)
        {
            menuOpenButtons[i].SetActive(false);
        }
    }

    public void OnUpgradeMenuOnePressed()
    {
        if (!upgradeMenu_1.activeInHierarchy)
        {
            upgradeMenu_1.SetActive(true);
            upgradeMenu_2.SetActive(false);
        }
    }

    public void OnUpgradeMenuTwoPressed()
    {
        if (!upgradeMenu_2.activeInHierarchy)
        {
            upgradeMenu_2.SetActive(true);
            upgradeMenu_1.SetActive(false);
        }
    }

    public void OnUpgradeMenuClosed()
    {
        upgradeMenu_1.SetActive(true);
        upgradeMenu_2.SetActive(false);
        upgradeMainMenu.SetActive(false);

        for (int i = 0; i < menuOpenButtons.Length; i++)
        {
            menuOpenButtons[i].SetActive(true);
        }

        if (defaultPauseOperations)
        {
            Time.timeScale = timeScale;
        }
    }

    public void AllowUpgrades()
    {
        menuOpenButtons[1].SetActive(true);
    }
    #endregion

    public void StartLevel()
    {
        startCanvas.SetActive(false);
        ingameCanvas.SetActive(true);
    }

    public void SetInGameScore(int score)
    {
        inGameScoreText.text = "" + score;
    }

    public void SetInGameScoreAsText(string scoreText)
    {
        inGameScoreText.text = scoreText;
    }


    public void DisplayScore(int score, int oldScore=0)
    {
        scoreText.DisplayScore(score, oldScore);
    }

    public void SetLevel(int level)
    {
        levelBarDisplay.SetLevel(level);
    }

    public void UpdateProgess(float progress)
    {
        levelBarDisplay.DisplayProgress(progress);
    }

    public void FinishLevel()
    {
        ingameCanvas.SetActive(false);
        finishCanvas.SetActive(true);
    }

    public void FailLevel()
    {
        ingameCanvas.SetActive(false);
        failCanvas.SetActive(true);
    }
    void InitStates()
    {
        ingameCanvas.SetActive(false);
        finishCanvas.SetActive(false);
        failCanvas.SetActive(false);
        startCanvas.SetActive(true);
    }

  
    public void OnRestartButtonPressed()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ChangeShownMoney();
    }

    #region MoneyRelated
    public void SetTargetScoreUI(int money, bool isInstant = false)
    {
        if (isInstant)
        {
            targetMoney = money;
            if(targetMoney != shownMoney)
            {
                ChangeMoneyInstantly();
            }
        }
        else
        {
            targetMoney = money;
            if (targetMoney != shownMoney)
            {
                canUpdateMoney = true;
            }
        }
    }

    void ChangeShownMoney()
    {
        if (canUpdateMoney)
        {
            shownMoney = Mathf.Lerp(shownMoney, targetMoney, Time.deltaTime * uiMoneyChangePerSecond);
            inGameScoreText.text = "" + shownMoney.ToString("0");

            if (Mathf.Abs(targetMoney - shownMoney) < lerpMargin)
            {
                canUpdateMoney = false;
            }
        }
    }

    void ChangeMoneyInstantly()
    {
        shownMoney = targetMoney;
        inGameScoreText.text = "" + shownMoney.ToString("0");
    }

    public void ShakeMoneyUIStart()
    {
        if (shakeMoney)
        {
            shakeMoney = false;
            moneyBox.transform.DOShakePosition(Time.deltaTime, shakePower, shakeVibrato, shakeRandomness).OnComplete(() =>
            {
                moneyBox.transform.DOSmoothRewind();
                moneyBox.transform.position = moneyBoxPosition;
                moneyBox.transform.localScale = moneyBoxScale;
                shakeMoney = true;
            });
        }
        
    }

    public void PunchMoneyUI()
    {
        if (punchMoney)
        {
            punchMoney = false;
            moneyBox.transform.DOPunchScale(punchVector, punchDuration).OnComplete(() =>
            {
                moneyBox.transform.DOSmoothRewind();
                moneyBox.transform.position = moneyBoxPosition;
                moneyBox.transform.localScale = moneyBoxScale;
                punchMoney = true;
            });
        }
        
    }

    #endregion

    #region UpgradeRelated
    //Menu1
    public void OnOilPumpUpgradeButtonPressed(int index)
    {
        OilPumpUpgradeButtonPressed(index);
    }

    public void OnElectricPumpUpgradeButtonPressed(int index)
    {
        ElectricPumpUpgradeButtonPressed(index);
    }

    public void UpgradeMenu1_UpdateOilStack(int[] upgradeLevels, int[] upgradeCosts)
    {
        for(int i = 0; i < oilPumpLevels.Length && i < upgradeLevels.Length; i++)
        {
            oilPumpLevels[i].text = "Capacity: " + upgradeLevels[i];
            oilPumpCosts[i].text = "" + upgradeCosts[i];
        }
    }

    public void UpgradeMenu1_UpdateElectricStack(int[] upgradeLevels, int[] upgradeCosts)
    {
        for(int i = 0; i < electricPumpLevels.Length && i < upgradeLevels.Length; i++)
        {
            electricPumpLevels[i].text = "Capacity: " + upgradeLevels[i];
            electricPumpCosts[i].text = "" + upgradeCosts[i];
        }
    }
    //Menu2
    public void OnOilSpeedBuyButtonPressed()
    {
        OilSpeedBuyButtonPressed();
    }

    public void OnOilStackBuyButtonPressed()
    {
        OilStackBuyButtonPressed();
    }

    public void OnElectricSpeedBuyButtonPressed()
    {
        ElectricSpeedBuyButtonPressed();
    }

    public void OnElectricStackBuyButtonPressed()
    {
        ElectricStackBuyButtonPressed();
    }

    public void UpgradeMenu2_UpdateOilSpeed(int cost, int level)
    {
        oilSpeedLevel.text = "Speed - Lvl:" + level;
        oilSpeedCost.text = "" + cost;
    }

    public void UpgradeMenu2_UpdateElectricSpeed(int cost, int level)
    {
        electricSpeedLevel.text = "Speed - Lvl:" + level;
        electricSpeedCost.text = "" + cost;
    }

    public void UpgradeMenu2_UpdateOilStack(int cost, int level)
    {
        oilStackLevel.text = "Stack - Lvl:" + level;
        oilStackCost.text = "" + cost;
    }

    public void UpgradeMenu2_UpdateElectricStack(int cost, int level)
    {
        electricStackLevel.text = "Stack - Lvl:" + level;
        electricStackCost.text = "" + cost;
    }
    #endregion

}
