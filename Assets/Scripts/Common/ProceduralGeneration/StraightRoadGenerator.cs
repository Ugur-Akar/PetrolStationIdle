using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StraightRoadGenerator : MonoBehaviour
{
    //Settings
    public float partLength;
    public bool spawnAtStart;
    // Connections
    public Transform roadsParent;
    public GameObject[] roadPrefabs;
    // State Variables

    // Start is called before the first frame update
    void Start()
    {
        //InitConnections();
        //InitState();
    }
    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public  GameObject[] GenerateRoads(int numberOfParts)
    {
        GameObject[] roadGOs = new GameObject[numberOfParts];
        Vector3 currentPosition = roadsParent.transform.position;
        for(int i=0; i<numberOfParts; i++)
        {
            roadGOs[i] = Instantiate(roadPrefabs[i % roadPrefabs.Length], currentPosition,Quaternion.identity, roadsParent);
            currentPosition += roadsParent.forward * partLength;
        }
        return roadGOs;
    }
}

