using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class SourcePump : MonoBehaviour
{
    public bool isTask = false;
    [Header("Collectible")]
    public GameObject collectible;
    GameObject collectibleGO;
    public Transform createPoint;
    public int collectibleLimitPerRow = 5;
    public int numberOfRows = 1;
    public float startingSecondsPerCollectible = 20;
    public float secondsPerCollectible = 4;
    public float xShiftPerCollectible = 1;
    public float yShiftPerRow;
    public float tweenDuration = 2;
    public List<GameObject> activeCollectibles;
    public GameObject[] collectibles;
    public GameObject nextGameObjectToGo;
    public Transform stackPoint;

    [Header("Others")]
    //Settings
    float xDefault;
    public static int maxNumberOfRows = 4;
    // Connections
    public DOTweenAnimation scaleAnimation;

    public PlayerManager pm;
    TaskManager taskManager;
    public Image filler;

    public event Action<GameObject> EnabledEvent;
    // State Variables
    float timer = 0;

    public int collectibleLimit;
    public int sourceCount = 0;
    //int index = 0;

    public List<Vector3> stackPositions;
    
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
        if(collectible.TryGetComponent<BarrelTweener>(out BarrelTweener bt))
        {
            if (ES3.KeyExists("oilNumberOfRows"))
            {
                numberOfRows = ES3.Load<int>("oilNumberOfRows");
                if(numberOfRows > maxNumberOfRows)
                {
                    numberOfRows = maxNumberOfRows;
                    ES3.Save("oilNumberOfRows", numberOfRows);
                }
            }
        }
        else
        {
            if (ES3.KeyExists("electricNumberOfRows"))
            {
                numberOfRows = ES3.Load <int>("electricNumberOfRows");
                if(numberOfRows > maxNumberOfRows)
                {
                    ES3.Save("electricNumberOfRows", numberOfRows);
                }
            }
        }


        if (ES3.KeyExists(nameof(numberOfRows)))
        {
            numberOfRows = ES3.Load<int>(nameof(numberOfRows));
            if(numberOfRows > maxNumberOfRows)
            {
                numberOfRows = maxNumberOfRows;
                ES3.Save(nameof(numberOfRows), numberOfRows);
            }
        }

        collectibleLimit = numberOfRows * collectibleLimitPerRow;
        stackPositions = new List<Vector3>();
        collectibles = new GameObject[maxNumberOfRows * collectibleLimitPerRow];

        SetStackPositions();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(activeCollectibles.Count < collectibleLimit)
        {
            filler.fillAmount = Mathf.InverseLerp(0, secondsPerCollectible, timer);
        }

        if(timer >= secondsPerCollectible && activeCollectibles.Count < collectibleLimit)
        {
            CreateCollectible();
            timer = 0;            
        }

    }

    void CreateCollectible()
    {
        Invoke(nameof(AddCollectible), tweenDuration);
        collectibleGO = Instantiate(collectible, createPoint.position, Quaternion.Euler(new Vector3(-90,0,0)), transform);
        if(collectibleGO.TryGetComponent<BarrelTweener>(out BarrelTweener barrelTweener))
        {
            collectibleGO.transform.localScale = Vector3.one * barrelTweener.startingScale;
            barrelTweener.creationTweenDuration = tweenDuration;

            for (int i = 0; i < collectibles.Length; i++)
            {
                if (collectibles[i] == null)
                {
                    collectibles[i] = collectibleGO;
                    if(i > 14)
                    {
                        Debug.Log(i);
                        Debug.Break();
                    }
                    barrelTweener.targetPosition = stackPositions[i];
                    break;
                }
            }

            barrelTweener.StartTweens();
        }
        else if(collectibleGO.TryGetComponent<BatteryTweener>(out BatteryTweener batteryTweener))
        {
            
            collectibleGO.transform.localScale = Vector3.one * batteryTweener.startingScale;
            batteryTweener.totalDuration = tweenDuration;

            for (int i = 0; i < collectibles.Length; i++)
            {
                if (collectibles[i] == null)
                {
                    collectibles[i] = collectibleGO;
                    batteryTweener.targetPosition = stackPositions[i];
                    break;
                }
            }

            batteryTweener.StartTweens();
        }
              
    }

    void AddCollectible()
    {
        activeCollectibles.Add(collectibleGO);
        nextGameObjectToGo = activeCollectibles[activeCollectibles.Count - 1];
    }


    void SetStackPositions()
    {
        for(int i = 0; i < maxNumberOfRows; i++)
        {
            for(int j = 0; j < collectibleLimitPerRow; j++)
            {
                Vector3 vec = new Vector3(stackPoint.position.x + (j * xShiftPerCollectible), stackPoint.position.y + (i * yShiftPerRow), stackPoint.position.z);
                stackPositions.Add(vec);
            }
        }
    }

    void FixBarrelPositions()
    {
        for(int i = 0; i < activeCollectibles.Count; i++)
        {
            activeCollectibles[i].transform.position = stackPositions[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(pm == null)
            {
                pm = other.GetComponent<PlayerManager>();
            }

            scaleAnimation.DORestart();

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

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            scaleAnimation.DOPause();
            scaleAnimation.DOSmoothRewind();
        }
    }


    public void CollectibleTaken()
    {
        for(int i = 0; i < collectibles.Length; i++)
        {
            if(nextGameObjectToGo == collectibles[i])
            {
                collectibles[i] = null;
            }
        }
        activeCollectibles.Remove(nextGameObjectToGo);
        if(activeCollectibles.Count > 0)
        {
            nextGameObjectToGo = activeCollectibles[activeCollectibles.Count - 1];
        }
        else
        {
            nextGameObjectToGo = null;
        }

    }

    public void NewSourceUnlocked(bool o_or_e)
    {
        
        if (o_or_e)
        {
            if (collectible.CompareTag("Barrel"))
            {
                sourceCount++;
                secondsPerCollectible = startingSecondsPerCollectible / sourceCount;
            }
        }
        else
        {
            if (collectible.CompareTag("Battery"))
            {
                sourceCount++;
                secondsPerCollectible = startingSecondsPerCollectible / sourceCount;
            }
        }
    }

    public void SetCollectibleDelay(int numberOfSources = 0)
    {
        sourceCount = numberOfSources;

        if(numberOfSources == 0)
        {
            secondsPerCollectible = float.MaxValue;
        }
        else
        {
            secondsPerCollectible = startingSecondsPerCollectible / sourceCount;
        }

    }

    public void OnStackLimitIncreased()
    {
        if(numberOfRows < maxNumberOfRows)
        {
            numberOfRows++;
            if(collectible.TryGetComponent<BarrelTweener>(out BarrelTweener bt))
            {
                ES3.Save("oilNumberOfRows", numberOfRows);
            }
            else
            {
                ES3.Save("electricNumberOfRows", numberOfRows);
            }

            collectibleLimit = numberOfRows * collectibleLimitPerRow;
        }
    }



    private void OnEnable()
    {
        EnabledEvent?.Invoke(gameObject);
    }
}

