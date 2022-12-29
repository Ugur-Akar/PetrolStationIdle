using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;


public class CarSpawnManager : MonoBehaviour
{
    [Header("Petrol")]
    public List<SplineComputer> petrolSplines;
    public List<PetrolCarPool> petrolCarPools;

    [Header("Electric")]
    public List<SplineComputer> electricSplines;
    public List<ElectricCarPool> electricCarPools;
    
    //Settings
    [Header("Settings")]
    public float timeIntervalBetweenCarSpawns = 3;
    public float intervalDeviation = 0.5f;
    public int maxAmountOfCarsPerRoad = 3;
    [Range(0.0f, 1.0f)]
    public float petrol_electricWeight = 0.5f;
    // Connections

    // State Variables
    public bool canSpawn = false;
    // Start is called before the first frame update
    void Awake()
	{
		//InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections(){
    }
    void InitState(){

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            SpawnPetrolCar();
            
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnElectricCar();
        }
        
        if (canSpawn)
        {
            SpawnCar();
        }
    }

    void SpawnPetrolCar()
    {
        int oldChoice = -1;
        bool cont = false;
        while (!cont)
        {
            int choice = Random.Range(0, petrolCarPools.Count);
            if(petrolCarPools.Count == 0)
            {
                cont = true;
            }
            else if (choice != oldChoice)
            {
                oldChoice = choice;
                cont = petrolCarPools[choice].SpawnCar();
                if (cont)
                {
                    oldChoice = -1;
                }
            }

        }
    }

    void SpawnElectricCar()
    {
        int oldChoice = -1;
        bool cont = false;
        while (!cont)
        {
            int choice = Random.Range(0, electricCarPools.Count);
            if (electricCarPools.Count == 0)
            {
                cont = true;
            }
            else if (choice != oldChoice)
            {
                oldChoice = choice;
                cont = electricCarPools[choice].SpawnCar();
                if (cont)
                {
                    oldChoice = -1;
                }
            }
            
        }
    }


    public void SetUpCarSpawner(List<SplineComputer> petrol, List<SplineComputer> electric, int petrolIndex, int electricIndex)
    {
        for(int i = 0; i < petrol.Count; i++)
        {
            if(i <= petrolIndex)
            {
                petrolSplines.Add(petrol[i]);
                petrolCarPools.Add(petrol[i].GetComponent<PetrolCarPool>());
            }


            if(i <= electricIndex)
            {
                electricSplines.Add(electric[i]);
                electricCarPools.Add(electric[i].GetComponent<ElectricCarPool>());
            }

        }
    }

    public void Unlocked(GameObject go)
    {
     
        if (go.TryGetComponent<PetrolCarPool>(out PetrolCarPool pcp))
        {
            petrolSplines.Add(pcp.GetComponent<SplineComputer>());
            petrolCarPools.Add(pcp);
        }
        else if (go.TryGetComponent<ElectricCarPool>(out ElectricCarPool ecp))
        {
            electricSplines.Add(ecp.GetComponent<SplineComputer>());
            electricCarPools.Add(ecp);
        }

    }

    void SpawnCar()
    {
        canSpawn = false;
        float pe = UnityEngine.Random.Range(0f, 1f);
        if(pe < petrol_electricWeight)
        {
            SpawnPetrolCar();
        }
        else
        {
            SpawnElectricCar();
        }

        Invoke(nameof(EnableSpawn), timeIntervalBetweenCarSpawns + UnityEngine.Random.Range(-intervalDeviation, intervalDeviation));
    }

    void EnableSpawn()
    {
        canSpawn = true;
    }

}

