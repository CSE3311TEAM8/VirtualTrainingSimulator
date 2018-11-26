using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDoorDestruction : MonoBehaviour {


    public GameObject mainDoor;
    void OnCollisionEnter(Collision col)
    {
        Destroy(mainDoor);
    }
}
