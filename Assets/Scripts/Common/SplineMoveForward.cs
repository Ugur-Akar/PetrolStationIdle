using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dreamteck.Splines;
using System;


public class SplineMoveForward : MonoBehaviour
{
    //Settings
    [SerializeField]
    public float speed;
    // Connections
    [SerializeField]
    public SplineFollower follower;
    public event Action PlayerMove;
    // State Variables
    
    // Start is called before the first frame update
    void Start()
    {
        InitConnections();
        //InitState();
    }
    void InitConnections(){
        follower = GetComponent<SplineFollower>();
    }
    void InitState(){
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            follower.motion.offset = new Vector2(follower.motion.offset.x + speed * Time.deltaTime, follower.motion.offset.y);
            //PlayerMove();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            follower.motion.offset = new Vector2(follower.motion.offset.x - speed * Time.deltaTime, follower.motion.offset.y);
           // PlayerMove();
        }
    }
}
