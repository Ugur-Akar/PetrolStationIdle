using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class Money : MonoBehaviour
{
    //Settings
    public float moveToPumpDuration = 0.5f;
    public float moveToPlayerDuration = 0.3f;
    public float moveToLockDuration = 0.4f;
    public float yShift;
    // Connections
    public Vector3 targetPosition;
    public Vector3 defaultPosition;

    public event Action<GameObject> Done; 
    // State Variables
    public int index = 0;
    
    // Start is called before the first frame update
	void Awake()
	{
		//InitConnections();
	}
    void Start()
    {
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

    public void MoveToPosition(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition, moveToPumpDuration).SetEase(Ease.Linear);
    }

    public void MoveToPlayer(Transform player)
    {
        transform.DOMove(player.position, moveToPlayerDuration);
    }

    public void MoveToLock(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition, moveToLockDuration).SetEase(Ease.Linear).onComplete += DisableGameObject;
    }

    void DisableGameObject()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }
    }
}

