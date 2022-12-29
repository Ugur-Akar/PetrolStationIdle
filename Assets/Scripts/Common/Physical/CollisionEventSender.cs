using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Collider))]
public class CollisionEventSender : MonoBehaviour
{
    public Action<Collider, string> OnTriggerEnterEvent;
    public Action<Collision, string> OnCollisionEnterEvent;

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(name);
        OnTriggerEnterEvent?.Invoke(other, gameObject.tag);
    }

    public void OnCollisionEnter(Collision collision)
    {
        OnCollisionEnterEvent?.Invoke(collision, gameObject.tag);
    }
}
