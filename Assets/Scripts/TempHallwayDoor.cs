using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class TempHallwayDoor : NetworkBehaviour
{
    public GameObject temp;
    public GameObject mainDoor;

    float delay = 0;
    void OnCollisionEnter(Collision col)
    {
        
        Animation temper = temp.GetComponent<Animation>();
        temper["DoorRooms_open"].speed = .06f;
        temper.enabled = true;
        temper["DoorRooms_open"].speed = .06f;

        //Invoke(MyFunction, 5.0f);

        //temper["DoorMain_open"].speed = -.06f;

        //Destroy(mainDoor);
    }

    

}
