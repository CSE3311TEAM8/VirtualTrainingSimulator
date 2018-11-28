/*
 * Author: Gonzalo Munoz & John Tieken 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TempWideDoor : NetworkBehaviour
{

    public GameObject temp;
    float delay = 0;
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("DoorRoomsWide_open has been activated");
        Animation temper = temp.GetComponent<Animation>();
        temper["DoorRoomsWide_open"].speed = .06f;
        temper.enabled = true;
        temper["DoorRoomsWide_open"].speed = .06f;

        //Invoke(MyFunction, 5.0f);

        //temper["DoorMain_open"].speed = -.06f;
    }
}