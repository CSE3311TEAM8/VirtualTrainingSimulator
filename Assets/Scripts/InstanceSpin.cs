/*
 * Author: Gonzalo Munoz & John Tieken 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceSpin : MonoBehaviour {

    public GameObject GO;
    

    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Enable: " + GO.name);
        GO.SetActive(true);
    }
}
