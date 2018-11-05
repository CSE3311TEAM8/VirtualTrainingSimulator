using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class Temp : NetworkBehaviour
{
    public GameObject temp;
    float delay = 0;
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Block white");
        Animation temper = temp.GetComponent<Animation>();
        temper["DoorMain_open"].speed = .06f;
        temper.enabled = true;
        temper["DoorMain_open"].speed = .06f;

        //Invoke(MyFunction, 5.0f);

        //temper["DoorMain_open"].speed = -.06f;
    }

    
    
}
