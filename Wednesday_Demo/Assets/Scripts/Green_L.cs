using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Green_L : NetworkBehaviour {

	//public GameObject Yellow;
	//public GameObject Cyan;

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "RedR") 
		{
			//Vector3 spawnSpot = col.transform.position;
			//GameObject cubeSpawn = (GameObject)Instantiate (Yellow, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);
			Debug.Log ("HELLLLLLOOO                     GreenL HIT RedR");
		}
		if (col.gameObject.name == "BlueR") 
		{
			//Vector3 spawnSpot = col.transform.position;
			//GameObject cubeSpawn = (GameObject)Instantiate (Cyan, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);
			Debug.Log ("HELLLLLLOOO                     GreenL HIT  BLUER");
		}
	}
}
