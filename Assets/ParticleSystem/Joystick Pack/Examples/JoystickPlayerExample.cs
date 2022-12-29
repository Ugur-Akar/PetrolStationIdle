using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickPlayerExample : MonoBehaviour
{
    public float speed;
    public DynamicJoystick variableJoystick;
    

    public void Update()
    {
        Vector3 direction = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;

        if (direction != Vector3.zero)
        {
            transform.forward = direction;
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }       
        
    }
}