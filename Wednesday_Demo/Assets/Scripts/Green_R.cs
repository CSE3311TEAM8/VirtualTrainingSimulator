using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Green_R : NetworkBehaviour {

	public GameObject Yellow;
	public GameObject Cyan;
	public Text Score;

	public GameObject GreenR;
	public GameObject BlueL;
	public GameObject RedL;

	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.name == "RedL") 
		{
			int CurrentScore = int.Parse(Score.text);
			CurrentScore++;
			Debug.Log (CurrentScore);
			Score.text = CurrentScore.ToString();
			Vector3 spawnSpot = col.transform.position;
			GameObject cubeSpawn = (GameObject)Instantiate (Yellow, spawnSpot, transform.rotation);

			Destroy (col.gameObject);
			Destroy (this.gameObject);

			Vector3 spawnGreenR = new Vector3(-0.918F, 1.231F, 1.03F);
			Vector3 spawnRedL = new Vector3 (-0.602F, 1.231F, 0.605F);
			GameObject GreenRSpawn = (GameObject)Instantiate (GreenR, spawnGreenR, transform.rotation);
			GameObject RedLSpawn = (GameObject)Instantiate (RedL, spawnRedL, transform.rotation);

			Debug.Log ("HELLLLLLOOO                     GreenR HIT RedL");
		}
		if (col.gameObject.name == "BlueL") 
		{
			int CurrentScore = int.Parse(Score.text);
			CurrentScore++;
			Debug.Log (CurrentScore);
			Score.text = CurrentScore.ToString();

			Vector3 spawnSpot = col.transform.position;
			GameObject cubeSpawn = (GameObject)Instantiate (Cyan, spawnSpot, transform.rotation);
			Destroy (col.gameObject);
			Destroy (this.gameObject);

			Vector3 spawnGreenR = new Vector3(-0.918F, 1.231F, 1.03F);
			Vector3 spawnBlueL = new Vector3 (-0.602F, 1.231F, 0.419F);
			GameObject GreenRSpawn = (GameObject)Instantiate (GreenR, spawnGreenR, transform.rotation);
			GameObject BlueLSpawn = (GameObject)Instantiate (BlueL, spawnBlueL, transform.rotation);

			Debug.Log ("HELLLLLLOOO                     GreenR HIT  BLUEL");
		}
	}

}
