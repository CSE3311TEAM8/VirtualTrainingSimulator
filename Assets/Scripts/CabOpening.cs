/*
 * Author: Gonzalo Munoz & John Tieken 
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CabOpening : NetworkBehaviour
{
    public GameObject temp;
    float delay = 0;
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Block white");
        Animation temper = temp.GetComponent<Animation>();
        temper["KitchenSetDoorTwo_anim"].speed = .06f;
        temper.enabled = true;
        temper["KitchenSetDoorTwo_anim"].speed = .06f;

        //Invoke(MyFunction, 5.0f);

        //temper["DoorMain_open"].speed = -.06f;
    }



}
