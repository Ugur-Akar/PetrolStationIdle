using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FXTestManager : MonoBehaviour
{
    //Settings
    [Header("Ascending Text / Press A")]
    public string ascendingTextInput;
    // Connections
    public AscendingTextEffect ascendingTextEffect;
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
        if (Input.GetKeyDown(KeyCode.A))
        {
            ascendingTextEffect.DisplayText("+1000");
        }

       

    }
}

