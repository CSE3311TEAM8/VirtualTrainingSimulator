using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Blue_L : NetworkBehaviour {


	//public GameObject Magenta;
	//public GameObject Cyan;

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "GreenR") 
		{
			//Vector3 spawnSpot = col.transform.position;
			//GameObject cubeSpawn = (GameObject)Instantiate (Cyan, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);
			Debug.Log ("HELLLLLLOOO                     BlueL HIT GREENR");
		}
		if (col.gameObject.name == "RedR") 
		{
			//Vector3 spawnSpot = col.transform.position;
			//GameObject cubeSpawn = (GameObject)Instantiate (Magenta, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);
			Debug.Log ("HELLLLLLOOO                     BlueL HIT  RedR");
		}
	}
}
