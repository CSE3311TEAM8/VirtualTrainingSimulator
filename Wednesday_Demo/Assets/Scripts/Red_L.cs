using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Red_L : NetworkBehaviour {

	//public GameObject Yellow;
	//public GameObject Magenta;

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "GreenR") 
		{
			//Vector3 spawnSpot = col.transform.position;
			//GameObject cubeSpawn = (GameObject)Instantiate (Yellow, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);
			Debug.Log ("HELLLLLLOOO                     REDL HIT GREENR");
		}
		if (col.gameObject.name == "BlueR") 
		{
			//Vector3 spawnSpot = col.transform.position;
			//GameObject cubeSpawn = (GameObject)Instantiate (Magenta, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);
			Debug.Log ("HELLLLLLOOO                     REDL HIT  BLUER");
		}
	}
}
