using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RagdollManager : MonoBehaviour
{
    //Settings

    // Connections
    Rigidbody[] allRigidBodies;
    Collider[] allColliders;
    public Transform[] partsToDetach;
    // State Variables
    
    // Start is called before the first frame update
    void Start()
    {
        InitConnections();
        InitState();
    }
    void InitConnections(){
        allRigidBodies = GetComponentsInChildren<Rigidbody>();
        allColliders = GetComponentsInChildren<Collider>();
    }
    void InitState(){
        SetRagdollEnabled(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRagdollEnabled(bool enabled)
    {
        for(int i=0; i<allRigidBodies.Length; i++)
        {
            allRigidBodies[i].isKinematic = !enabled;
            
        }
        for(int i=0; i<allColliders.Length; i++)
        {
            allColliders[i].enabled = enabled;
        }
    }

}

