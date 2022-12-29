using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;


public class PetrolCarPool : MonoBehaviour
{
    //Settings
    public bool isInactive = false;
    public int amountToPool = 5;
    public float durationBetweenSpawns = 2;
    // Connections
    public GameObject[] carPrefabs;
    public List<GameObject> cars;
    public List<PetrolCar> petrolCars;
    public SplineComputer computer;
    // State Variables
    float timer = 0;
    bool canSpawn = true;
    // Start is called before the first frame update
	void Awake()
	{
        //InitConnections();
        
    }
    void Start()
    {
        //InitState();
        GameObject tmp;
        for (int i = 0; i < amountToPool; i++)
        {
            tmp = Instantiate(carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Length)], computer.GetPointPosition(0), Quaternion.identity,  transform);
            PetrolCar pc = tmp.GetComponent<PetrolCar>();
            pc.follower.spline = computer;
            pc.Stop();
            petrolCars.Add(pc);

            tmp.SetActive(false);
            cars.Add(tmp);
        }

        if (isInactive)
        {
            //gameObject.SetActive(false);
        }
    }
    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= durationBetweenSpawns)
        {
            canSpawn = true;
        }
    }

    GameObject GetPooledCar()
    {
        for (int i = 0; i < amountToPool; i++)
        {
            if (!cars[i].activeInHierarchy)
            {
                return cars[i];
            }
        }
        return null;
    }

    public bool SpawnCar()
    {
        GameObject car = GetPooledCar();
        if(car != null && canSpawn)
        {
            timer = 0;
            PetrolCar pc = petrolCars[cars.IndexOf(car)];
            car.transform.position = computer.GetPointPosition(0);
            pc.follower.SetPercent(0);
            car.SetActive(true);
            pc.Move();
            canSpawn = false;
            return true;
        }

        return false;
    }

   
}

