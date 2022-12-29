using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPlacer : MonoBehaviour, IConfigurableSpawnable
{
    //Settings
    // Connections

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

    public void Configure(float[] parameters)
    {
        //float minX = parameters[0];
        //float maxX = parameters[1];
        //float redIndex = parameters[2];
        //PlaceOnX(minX, maxX);
        //SetColorRed(redIndex);
    }

    public void PlaceOnX(float minX,float maxX)
    {
        float xPos = Random.Range(minX, maxX);
        transform.position = new Vector3(xPos, transform.position.y, transform.position.z);
    }

    public void SetColorRed(float redIndex)
    {
        Color currentColor = GetComponent<Renderer>().material.color;
        currentColor.r = redIndex;
        GetComponent<Renderer>().material.color = currentColor;
    }
}

