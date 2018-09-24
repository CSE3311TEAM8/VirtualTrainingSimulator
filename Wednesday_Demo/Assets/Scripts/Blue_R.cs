using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Blue_R : NetworkBehaviour {


	public GameObject Magenta;
	public GameObject Cyan;
	public Text Score;

	public GameObject BlueR;
	public GameObject GreenL;
	public GameObject RedL;

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "GreenL") 
		{
			int CurrentScore = int.Parse(Score.text);
			CurrentScore++;
			Debug.Log (CurrentScore);
			Score.text = CurrentScore.ToString();
			Vector3 spawnSpot = col.transform.position;
			GameObject cubeSpawn = (GameObject)Instantiate (Cyan, spawnSpot, transform.rotation);

			Destroy (col.gameObject);
			Destroy (this.gameObject);

			Vector3 spawnBlueR = new Vector3(-0.918F, 1.231F, 1.23F);
			Vector3 spawnGreenL = new Vector3 (-0.602F, 1.231F, 0.219F);
			GameObject BlueRSpawn = (GameObject)Instantiate (BlueR, spawnBlueR, transform.rotation);
			GameObject GreenLSpawn = (GameObject)Instantiate (GreenL, spawnGreenL, transform.rotation);

			Debug.Log ("HELLLLLLOOO                     BlueR HIT GREENL");
		}
		if (col.gameObject.name == "RedL") 
		{
			int CurrentScore = int.Parse(Score.text);
			CurrentScore++;
			Debug.Log (CurrentScore);
			Score.text = CurrentScore.ToString();

			Vector3 spawnSpot = col.transform.position;
			GameObject cubeSpawn = (GameObject)Instantiate (Magenta, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);

			Vector3 spawnBlueR = new Vector3(-0.918F, 1.231F, 1.23F);
			Vector3 spawnRedL = new Vector3 (-0.602F, 1.231F, 0.605F);
			GameObject BlueRSpawn = (GameObject)Instantiate (BlueR, spawnBlueR, transform.rotation);
			GameObject RedLSpawn = (GameObject)Instantiate (RedL, spawnRedL, transform.rotation);

			Debug.Log ("HELLLLLLOOO                     BlueR HIT  RedL");
		}
	}
		
}
