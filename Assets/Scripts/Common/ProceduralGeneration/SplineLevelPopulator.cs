using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Dreamteck.Splines;

public class SplineLevelPopulator : MonoBehaviour
{
    const int SLOT_WILL_BE_EMPTY = -1;

    //Settings
    public int numberOfElements = 10;
    public int initialNumberOfGoods = 1;
    public bool spawnAtStart;
    // Connections
    public SplineComputer spline;
    public Transform origin;
    public Vector3 offset;
    public SpawnableGroup[] spawnableGroups;
    public float spawnDistance;
    // State Variables
    public float[] integralProbabilities;

    // Start is called before the first frame update
    void Start()
    {
        Random.InitState(21313);
        //InitConnections();
        InitState();
    }
    void InitConnections(){
    }
    void InitState(){
        CalculateIntegralProbabilities();
        if(spawnAtStart)
            Populate(); // TODO: will be called from another script
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Populate()
    {
        float spawnPosition = 0;
        for (int i = 0; i < initialNumberOfGoods; i++)
        {
            Spawn(spawnPosition, true);
            spawnPosition += spawnDistance; // TODO: Randomize spawnDistance
        }
        for (int i = initialNumberOfGoods; i < numberOfElements; i++)
        {
            Spawn(spawnPosition, false);
            spawnPosition += spawnDistance; // TODO: Randomize spawnDistance
        }
    }

    
    void Spawn(float position, bool spawnPositive=false)
    {
        int randomPrefabIndex = SelectRandomSpawnable();
        if (spawnPositive)
            randomPrefabIndex = 0;

        if (randomPrefabIndex == SLOT_WILL_BE_EMPTY) return;

        //#if UNITY_EDITOR
        //GameObject spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(possiblePrefabs[randomPrefabIndex], parentTransforms[randomPrefabIndex]);
        //#else
        // GameObject spawnedObject = Instantiate(possiblePrefabs[randomPrefabIndex], parentTransforms[randomPrefabIndex]);
        SplineSample result = spline.Evaluate(position);
        SpawnableGroup selectedGroup = spawnableGroups[randomPrefabIndex];
        GameObject spawnedObject = Instantiate(selectedGroup.prefab, selectedGroup.parent);
        spawnedObject.transform.position = result.position + result.right * Random.Range(-3, 4);
        IConfigurableSpawnable configurator = spawnedObject.GetComponent<IConfigurableSpawnable>();
        configurator?.Configure(selectedGroup.configParameters);

//#endif
        
    }

    #region Random Selection Algorithms

    void CalculateIntegralProbabilities()
    {
        integralProbabilities = new float[spawnableGroups.Length];
        float currentIntegralProbability = 0.0f;
        for (int i = 0; i < integralProbabilities.Length; i++)
        {
            currentIntegralProbability += spawnableGroups[i].probability;
            integralProbabilities[i] = currentIntegralProbability;
        }
    }

    int SelectRandomSpawnable()
    {
        float dice = Random.Range(0, 1.0f);

        int currentSpawnableIndex = SLOT_WILL_BE_EMPTY;
        for (int j = 0; j < spawnableGroups.Length; j++)
        {
            if (dice < integralProbabilities[j])
            {
                currentSpawnableIndex = j;
                break;
            }
        }
        return currentSpawnableIndex;
    }

    #endregion

}

