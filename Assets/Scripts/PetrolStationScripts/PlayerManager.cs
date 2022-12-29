using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using MoreMountains.NiceVibrations;
using GameAnalyticsSDK;


public class PlayerManager : MonoBehaviour
{
    int mc = 0;
    [Header("CollectibleSettings")]
    public List<GameObject> stack;
    public GameObject nextObjectToAdd;
    public bool canCollect = true;
    public int stackLimit;

    [Header("StackingSettings")]
    public Transform stackPoint;
    Transform nextStackPoint;
    public Vector3 stackPointStartPosition;

    [Header("OilRefillSettings")]
    public int barrelCountInStack = 0;
    public bool canRefillOil = false;
    public GameObject barrelToGo;


    [Header("BatteryRechargeSettings")]
    public int batteryCountInStack = 0;
    public bool canRechargeBattery = false;
    public GameObject batteryToGo;

    [Header("ThrashSettings")]
    public Transform trash;
    public bool canThrowTrash = true;
    public GameObject nextObjectToThrow;
    public float trashJumpHeight = 1;
    public float trashDuration = 0.2f;

    //Settings
    public float speed = 5;
    public static int moneyValue = 5;
    // Connections
    public PlayerCollisionControl collisionControl;
    public Rigidbody rb;
    public DynamicJoystick joystick;
    public ObjectPool objectPool;
    public ArrowManager arrowManager;

    PlayerAnimationManager pam;
    SourcePump sourcePump;
    OilPump oilPump;
    ElectricPump electricPump;

    Transform unlockTransform;
    Unlock targetLock;
    Unlock previousTargetLock;
    MainSourceUnlocker mainSourceTargetLock;

    

    public event Action<bool> MoneyChangedEvent;
    public event Action ShakeMoneyUI;
    public event Action PunchMoneyUI;
    // State Variables
    public int money = 0;
    bool isCollecting = false;
    bool isRefillingOil = false;
    bool isRechargingBattery = false;
    bool isUnlocking = false;
    bool canUnlock = true;
    bool isThrowingTrash = false;
    bool moneySaver = false;

    // Start is called before the first frame update
    void Awake()
	{
		InitConnections();
	}
    void Start()
    {
        InitState();
    }
    void InitConnections(){
        pam = GetComponent<PlayerAnimationManager>();

        if (ES3.KeyExists(nameof(money)))
        {
            money = ES3.Load<int>(nameof(money));
        }
        else
        {
            money = 0;
        }

        if(money < 0)
        {
            money = 0;
        }

        MoneyChangedEvent(false);

        CollisionControlAttachMethodsToEvents();
    }
    void InitState(){
        stackPointStartPosition = stackPoint.transform.localPosition;
    }

    private void Update()
    {
        if (isCollecting)
        {
            Collect();
        }
        if (isRefillingOil)
        {
            RefillOil();
        }
        if (isRechargingBattery)
        {
            RechargeBattery();
        }
        if (isUnlocking)
        {
            Unlock();
        }
        if (isThrowingTrash)
        {
            ThrowTrash();
        }
        
        

        if (Input.GetKeyDown(KeyCode.A))
        {
            money += 1000;
            MoneyChangedEvent(true);
        }
        
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector3 direction = Vector3.forward * joystick.Vertical + Vector3.right * joystick.Horizontal;
       
        rb.velocity = direction * speed;
        if(rb.velocity != Vector3.zero)
        {
            transform.forward = rb.velocity;
            if(stack.Count > 0)
            {
                pam.CarryWalk();
            }
            else
            {
                pam.EmptyWalk();
            }
        }
        else
        {
            if(stack.Count > 0)
            {
                pam.CarryIdle();
            }
            else
            {
                pam.EmptyIdle();
            }
        }


    }


    #region CollisionControls
    void CollisionControlAttachMethodsToEvents()
    {
        collisionControl.OilMainStart += OilMainStart;
        collisionControl.OilMainStay += OilMainStay;
        collisionControl.OilMainExit += OilMainExit;

        collisionControl.ElectricMainStart += ElectricMainStart;
        collisionControl.ElectricMainStay += ELectricMainStay;
        collisionControl.ElectricMainExit += ElectricMainExit;

        collisionControl.OilPumpStart += OilPumpStart;
        collisionControl.OilPumpStay += OilPumpStay;
        collisionControl.OilPumpExit += OilPumpExit;

        collisionControl.ElectricPumpStart += ElectricPumpStart;
        collisionControl.ElectricPumpStay += ElectricPumpStay;
        collisionControl.ElectricPumpExit += ElectricPumpExit;

        collisionControl.EarnMoney += EarnMoney;
        collisionControl.Unlock += StartUnlocking;
        collisionControl.MainSourceUnlock += StartMainSourceUnlocking;
        collisionControl.StopUnlock += StopUnlocking;

        collisionControl.TrashEnter += TrashEnter;
        collisionControl.TrashExit += TrashExit;
    }

    #region OilMain
    void OilMainStart(SourcePump sp)
    {
        sourcePump = sp;
        isCollecting = true;
    }

    void OilMainStay()
    {
        
    }

    void OilMainExit()
    {
        isCollecting = false;
    }
    #endregion

    #region ElectricMain
    void ElectricMainStart(SourcePump sp)
    {
        sourcePump = sp;
        isCollecting = true;
    }

    void ELectricMainStay()
    {

    }

    void ElectricMainExit()
    {
        isCollecting = false;
    }

    #endregion

    #region OilPump

    void OilPumpStart(OilPump op)
    {
        oilPump = op;
        isRefillingOil = true;
    }

    void OilPumpStay()
    {

    }

    void OilPumpExit()
    {
        isRefillingOil = false;
    }

    #endregion

    #region ElectricPump

    void ElectricPumpStart(ElectricPump ep)
    {
        electricPump = ep;
        isRechargingBattery = true;
    }

    void ElectricPumpStay()
    {

    }

    void ElectricPumpExit()
    {
        isRechargingBattery = false;
    }

    #endregion

    #endregion

    #region Collect
    void Collect()
    {
        if (canCollect)
        {
            if(nextObjectToAdd == null && sourcePump.nextGameObjectToGo != null && stack.Count < stackLimit)
            {
                nextObjectToAdd = sourcePump.nextGameObjectToGo;
                sourcePump.CollectibleTaken();
                nextObjectToAdd.transform.SetParent(transform);
                MMVibrationManager.Vibrate();

                if (nextObjectToAdd.TryGetComponent<BarrelTweener>(out BarrelTweener barrelTweener))
                {
                    barrelCountInStack++;
                    barrelTweener.MoveToPlayer(stackPoint.localPosition);
                    nextStackPoint = barrelTweener.topPoint;
                    barrelTweener.ReachedPosition += EnableCollecting;
                    barrelTweener.ReachedPump += BarrelReachedPump;

                }
                else if(nextObjectToAdd.TryGetComponent<BatteryTweener>(out BatteryTweener batteryTweener))
                {
                    batteryCountInStack++;
                    batteryTweener.MoveToPlayer(stackPoint.localPosition);
                    nextStackPoint = batteryTweener.topPoint;
                    batteryTweener.ReachedPosition += EnableCollecting;
                    batteryTweener.ReachedPump += BatteryReachedPump;
                }


                canCollect = false;
            }
            else
            {
                return;
            }
        }
    }

    void EnableCollecting()
    {
        stackPoint.position = nextStackPoint.position;
        nextStackPoint = null;
        canCollect = true;
        stack.Add(nextObjectToAdd);
        nextObjectToAdd = null;
    }
    #endregion

    #region RefillOil
    void RefillOil()
    {
        if(barrelCountInStack > 0 && oilPump.canRefill && canRefillOil)
        {
            for(int i = stack.Count - 1; i >= 0; i--)
            {
                if(stack[i].CompareTag("Barrel"))
                {
                    MMVibrationManager.Vibrate();
                    canRefillOil = false;
                    barrelCountInStack--;
                    barrelToGo = stack[i];
                    stack[i].transform.parent = null;
                    BarrelTweener bt = stack[i].GetComponent<BarrelTweener>();                    
                    bt.MoveToPump(oilPump.tweenTarget.position);
                    stack.RemoveAt(i);
                    FixStackPositions();
                    break;
                }
            }
        }
    }

    void BarrelReachedPump()
    {
        oilPump.AddFuel(barrelToGo);
        canRefillOil = true;
    }

    #endregion

    #region RechargeBattery

    void RechargeBattery()
    {
        if(batteryCountInStack > 0 && electricPump.canCharge && canRechargeBattery)
        {
            for (int i = stack.Count - 1; i >= 0; i--)
            {
                if (stack[i].CompareTag("Battery"))
                {
                    MMVibrationManager.Vibrate();
                    canRechargeBattery = false;
                    batteryCountInStack--;
                    batteryToGo = stack[i];
                    BatteryTweener bt = stack[i].GetComponent<BatteryTweener>();
                    bt.MoveToPump(electricPump.tweenTarget.position);
                    stack.RemoveAt(i);
                    FixStackPositions();
                    break;
                }
            }
        }
    }

    void BatteryReachedPump()
    {
        electricPump.AddEnergy(batteryToGo);
        canRechargeBattery = true;
    }

    #endregion

    #region Unlock
    void Unlock()
    {
        bool lockBool = false;
        if (targetLock != null)
        {
            lockBool = targetLock.canUnlock;
        }
        else if(mainSourceTargetLock != null)
        {
            lockBool = mainSourceTargetLock.canUnlock;
        }

        if (money > 0 && canUnlock && targetLock.canUnlock)
        {
            MMVibrationManager.Vibrate();
            money -= moneyValue;
            MoneyChangedEvent(false);
            float delay = objectPool.SendUnlockMoney(unlockTransform.position);
            canUnlock = false;
            Invoke(nameof(InvokeUnlock), delay * 2);
            Invoke(nameof(EnableUnlock), delay);
        }

        if(money <= 0)
        {
            ShakeMoneyUI();
        }
    }

    void InvokeUnlock()
    {
        if(targetLock != null)
        {
            moneySaver = targetLock.MoneyArrived();
        }
        else
        {
            if(previousTargetLock != null)
                moneySaver = previousTargetLock.MoneyArrived();
        }

        if (moneySaver)
        {
            money += moneyValue;
            MoneyChangedEvent(true);
        }
    }

    void StartUnlocking(Unlock unlock)
    {
        isUnlocking = true;
        targetLock = unlock;
        unlockTransform = unlock.transform;
    }

    void EnableUnlock()
    {
        canUnlock = true;
    }

    void StopUnlocking()
    {
        previousTargetLock = targetLock;
        isUnlocking = false;
        targetLock = null;
        mainSourceTargetLock = null;
        unlockTransform = null;
    }

    void StartMainSourceUnlocking(MainSourceUnlocker msu)
    {
        isUnlocking = true;
        mainSourceTargetLock = msu;
        unlockTransform = msu.transform;
    }

    #endregion

    #region Trash
    void ThrowTrash()
    {
        if (canThrowTrash)
        {
            
            if(nextObjectToThrow == null && trash != null && stack.Count > 0)
            {
                nextObjectToThrow = stack[stack.Count - 1];
                canThrowTrash = false;
                MMVibrationManager.Vibrate();

                if (stack[stack.Count - 1].CompareTag("Barrel"))
                {
                    barrelCountInStack--;
                }
                else
                {
                    batteryCountInStack--;
                }

                nextObjectToThrow.transform.SetParent(null);
                nextObjectToThrow.transform.DOJump(trash.position, trashJumpHeight, 1, trashDuration).OnComplete(() =>
                {
                    Destroy(nextObjectToThrow);
                    EnableThrowing();
                });

                stack.RemoveAt(stack.Count - 1);
                FixStackPositions();
            }

        }
    }

    void TrashEnter(Transform trashTransform)
    {
        trash = trashTransform;
        isThrowingTrash = true;
    }

    void TrashExit()
    {
        isThrowingTrash = false;
    }

    void EnableThrowing()
    {
        nextObjectToThrow = null;
        nextStackPoint = null;
        canThrowTrash = true;
    }

    #endregion
    void FixStackPositions()
    {
        stackPoint.localPosition = stackPointStartPosition;
        for(int i = 0; i < stack.Count; i++)
        {
            stack[i].transform.localPosition = stackPoint.localPosition;

            if (stack[i].CompareTag("Barrel"))
            {
                nextStackPoint = stack[i].GetComponent<BarrelTweener>().topPoint;
            }
            else if (stack[i].CompareTag("Battery"))
            {
                nextStackPoint = stack[i].GetComponent<BatteryTweener>().topPoint;
            }

            stackPoint.position = nextStackPoint.position;
            nextStackPoint = null;
        }
    }

    void EarnMoney()
    {
        MMVibrationManager.Vibrate();
        money += moneyValue;
        MoneyChangedEvent(false);
        PunchMoneyUI();
    }

}

