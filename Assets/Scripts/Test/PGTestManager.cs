using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PGTestManager : MonoBehaviour
{
    //Settings
    public int numberOfTestRoads = 20;
    // Connections
    public StraightRoadGenerator roadGenerator;
    public StraightLevelPopulator populator;
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
        CheckRoadGenerationButton();
    }

    void CheckRoadGenerationButton()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            roadGenerator.GenerateRoads(numberOfTestRoads);
            populator.Populate();
        }
    }
}

