using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CharacterAnimator : MonoBehaviour
{
    const float FULL_TURN_ANGLE = 360;

    //Settings
    public float turnAroundTime = 1.0f;
    // Connections
    public Animator[] animators;
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

    public void SetTrigger(string trigger)
    {
        foreach (Animator animator in animators)
        {

            animator.SetTrigger(trigger);
        }
    }

    public void TurnAround()
    {
        foreach (Animator animator in animators)
        {

            animator.transform.DORotate(Vector3.up * FULL_TURN_ANGLE, turnAroundTime, RotateMode.FastBeyond360);
        }
    }
}

