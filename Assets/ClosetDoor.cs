using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class ClosetDoor : NetworkBehaviour
{

    public GameObject temp;
    float delay = 0;
    void OnCollisionEnter(Collision col)
    {
        Debug.Log("Block white");
        Animation temper = temp.GetComponent<Animation>();
        temper["DoorClosetNarrow_open"].speed = .06f;
        temper.enabled = true;
        temper["DoorClosetNarrow_open"].speed = .06f;

        //Invoke(MyFunction, 5.0f);

        //temper["DoorMain_open"].speed = -.06f;
    }
}
