using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;


public class BarrelTweener : MonoBehaviour
{
    //Settings
    public float startingScale = 0.1f;
    public float creationTweenDuration = 2;
    public float toPlayerTweenDuration = 0.3f;
    public float toPumpTweenDuration = 0.3f;
    public float targetScale = 0.7f;
    public Vector3 targetPosition;
    // Connections
    public Transform topPoint;
    public Transform bottomPoint;

    public event Action ReachedPosition;
    public event Action ReachedPump;
    // State Variables
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

    public void StartTweens()
    {
        transform.DOJump(targetPosition, 1, 0, creationTweenDuration).SetEase(Ease.OutFlash);
        transform.DORotate(new Vector3(0, 90, 0), creationTweenDuration);
        transform.DOScale(Vector3.one * targetScale, creationTweenDuration);
    }

    public void MoveToPlayer(Vector3 target)
    {
        targetPosition = target;
        transform.DOLocalMove(targetPosition, toPlayerTweenDuration).SetEase(Ease.Linear).onComplete += OnPlayerReached;
    }

    public void MoveToPump(Vector3 target)
    {
        transform.DOJump(target, 0, 1, toPumpTweenDuration).onComplete += OnPumpReached;
    }

    void OnPumpReached()
    {
        ReachedPump();
    }

    void OnPlayerReached()
    {
        ReachedPosition();
    }
}

