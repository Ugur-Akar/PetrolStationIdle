using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;

public class SplineRoadGenerator : MonoBehaviour
{
    //Settings
    
    // Connections
    
    // State Variables
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject splineGO = new GameObject("splineGO");

        //Add a Spline Computer component to this object
        SplineComputer spline = splineGO.AddComponent<SplineComputer>();
        //Create a new array of spline points
        SplinePoint[] points = new SplinePoint[5];
        //Set each point's properties
        for (int i = 0; i < points.Length; i++)
        {
            points[i] = new SplinePoint();
            points[i].position = Vector3.forward * i;
            points[i].normal = Vector3.up;
            points[i].size = 1f;
            points[i].color = Color.white;
        }
        //Write the points to the spline
        spline.SetPoints(points);

    }

    //void GenerateRandomSpline(int numberOfPoints, float pointSpawnDistance,)

    void InitConnections(){
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

