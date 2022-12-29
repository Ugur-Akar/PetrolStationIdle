using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class PlayerCollisionControl : MonoBehaviour
{
    //Settings

    // Connections
    public event Action<SourcePump> OilMainStart;
    public event Action OilMainStay;
    public event Action OilMainExit;

    public event Action<OilPump> OilPumpStart;
    public event Action OilPumpStay;
    public event Action OilPumpExit;

    public event Action<SourcePump> ElectricMainStart;
    public event Action ElectricMainStay;
    public event Action ElectricMainExit;

    public event Action<ElectricPump> ElectricPumpStart;
    public event Action ElectricPumpStay;
    public event Action ElectricPumpExit;

    public event Action<Transform> TrashEnter;
    public event Action TrashExit;


    public event Action EarnMoney;
    public event Action<Unlock> Unlock;
    public event Action<MainSourceUnlocker> MainSourceUnlock;
    public event Action StopUnlock;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("OilMain"))
        {
            OilMainStart(other.GetComponent<SourcePump>());
        }
        if (other.CompareTag("ElectricMain"))
        {
            ElectricMainStart(other.GetComponent<SourcePump>());
        }
        if (other.CompareTag("OilPump"))
        {
            OilPumpStart(other.GetComponent<OilPump>());
        }
        if (other.CompareTag("ElectricPump"))
        {
            ElectricPumpStart(other.GetComponent<ElectricPump>());
        }
        if (other.CompareTag("Trash"))
        {
            TrashEnter(other.transform);
        }

        ///////////////////////////////////////////////////////////

        if (other.CompareTag("Money"))
        {
            EarnMoney();
        }
        if (other.CompareTag("Unlock"))
        {
            Unlock(other.GetComponent<Unlock>());
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("OilMain"))
        {
            OilMainStay();
        }
        if (other.CompareTag("ElectricMain"))
        {
            ElectricMainStay();
        }
        if (other.CompareTag("OilPump"))
        {
            OilPumpStay();
        }
        if (other.CompareTag("ElectricPump"))
        {
            ElectricPumpStay();
        }
      
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("OilMain"))
        {
            OilMainExit();
        }
        if (other.CompareTag("ElectricMain"))
        {
            ElectricMainExit();
        }
        if (other.CompareTag("OilPump"))
        {
            OilPumpExit();
        }
        if (other.CompareTag("ElectricPump"))
        {
            ElectricPumpExit();
        }
        if (other.CompareTag("Trash"))
        {
            TrashExit();
        }

        /////////////////////////////////////////////////////////

        if (other.CompareTag("Unlock"))
        {
            StopUnlock();
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        
    }

    public void CallStopUnlock()
    {
        StopUnlock?.Invoke();
    }
}

