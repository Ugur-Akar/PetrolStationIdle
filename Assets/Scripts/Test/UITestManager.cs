using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UITestManager : MonoBehaviour
{
    //Settings
    public int testScoreUnit = 100;
    public float testRunPercent = 0.1f;
    // Connections
    public UIManager uiManager;
    // State Variables
    float currentProgress;
    int inLevelScore;
    int levelIndex;
    bool levelComplete;
    // Start is called before the first frame update
    void Start()
    {
        InitConnections();
        InitState();
    }
    void InitConnections(){
        uiManager.OnLevelStart += OnStartButtonPressed;
        uiManager.OnLevelRestart += OnRestartLevelButtonPressed;
        uiManager.OnNextLevel += OnNextLevelButtonPressed;

    }
    void InitState(){
        currentProgress = 0;
        inLevelScore = 0;
        LoadLevel();
        levelComplete = false;
    }

    void LoadLevel()
    {
        levelIndex = PlayerPrefs.GetInt("levelIndex", 0);
        // Load the level prefab here
        uiManager.SetLevel(levelIndex);
        
    }

    void OnStartButtonPressed()
    {
        // Set user input enable
        Debug.Log("Start pressed");
    }

    void OnRestartLevelButtonPressed()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void OnNextLevelButtonPressed()
    {
        levelIndex++;
        PlayerPrefs.SetInt("levelIndex", levelIndex);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    // Update is called once per frame
    void Update()
    {
        CheckRunButton();
        CheckFailButton();
        CheckScoreButton();
        CheckAndUpdateProgress();
    }

    private void CheckRunButton()
    {
        if (Input.GetKey(KeyCode.R))
        {
            currentProgress += testRunPercent * Time.deltaTime;
            if (currentProgress > 1 && !levelComplete)
            {
                levelComplete = true;
                OnLevelCompleted();
            }
        }
    }

    

    private void CheckFailButton()
    {
        if (Input.GetKey(KeyCode.X))
        {
            uiManager.FailLevel();
        }
    }

    private void CheckScoreButton()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            inLevelScore += testScoreUnit;
            uiManager.SetInGameScore(inLevelScore);
        }
    }

    void CheckAndUpdateProgress()
    {
        uiManager.UpdateProgess(currentProgress);
    }

    public void MyPing(int x)
    {
        Debug.Log("my ping " + x);
    }

    public void OnSoldierDied(string soldierName, int id)
    {
        Debug.Log(soldierName + " sehit oldu, id: " + id);
    }

    private void OnLevelCompleted()
    {
        currentProgress = 1;
        uiManager.FinishLevel();
        int oldScore = PlayerPrefs.GetInt("score", 0);
        PlayerPrefs.SetInt("score", oldScore + inLevelScore);
        uiManager.DisplayScore(oldScore + inLevelScore, oldScore);
    }
}

