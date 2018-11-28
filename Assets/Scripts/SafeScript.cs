/*
 * Author: Gonzalo Munoz & John Tieken 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeScript : MonoBehaviour {

    public GameObject temp;
    float delay = 0;
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Block white");
        Animation temper = temp.GetComponent<Animation>();
        temper["Safe_open"].speed = .06f;
        temper.enabled = true;
        temper["Safe_open"].speed = .06f;

        //Invoke(MyFunction, 5.0f);

        //temper["DoorMain_open"].speed = -.06f;
    }
}
