using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class Red_R : NetworkBehaviour {

	public GameObject Yellow;
	public GameObject Magenta;

	public GameObject RedR;
	public GameObject GreenL;
	public GameObject BlueL;

	public Text Score;
 
	void OnCollisionEnter(Collision col)
	{
		
		if (col.gameObject.name == "GreenL") 
		{
			int CurrentScore = int.Parse(Score.text);
			CurrentScore++;
			Debug.Log (CurrentScore);
			Score.text = CurrentScore.ToString();

			Vector3 spawnSpot = col.transform.position;
			GameObject cubeSpawn = (GameObject)Instantiate (Yellow, spawnSpot, transform.rotation);

			Destroy (col.gameObject);
			Destroy (this.gameObject);

			Vector3 spawnRedR = new Vector3(-0.918F, 1.231F, 1.416F);
			Vector3 spawnGreenL = new Vector3 (-0.602F, 1.231F, 0.219F);
			GameObject RedRSpawn = (GameObject)Instantiate (RedR, spawnRedR, transform.rotation);
			GameObject GreenLSpawn = (GameObject)Instantiate (GreenL, spawnGreenL, transform.rotation);



			Debug.Log ("HELLLLLLOOO                     REDR HIT GREENL");
		}
		if (col.gameObject.name == "BlueL") 
		{
			int CurrentScore = int.Parse(Score.text);
			CurrentScore++;
			Debug.Log (CurrentScore);
			Score.text = CurrentScore.ToString ();
				
			Vector3 spawnSpot = col.transform.position;
			GameObject cubeSpawn = (GameObject)Instantiate (Magenta, spawnSpot, transform.rotation);

			Destroy (col.gameObject);
			Destroy (this.gameObject);

			Vector3 spawnRedR = new Vector3(-0.918F, 1.231F, 1.416F);
			Vector3 spawnBlueL = new Vector3 (-0.602F, 1.231F, 0.419F);
			GameObject RedRSpawn = (GameObject)Instantiate (RedR, spawnRedR, transform.rotation);
			GameObject BlueLSpawn = (GameObject)Instantiate (BlueL, spawnBlueL, transform.rotation);

			Debug.Log ("HELLLLLLOOO                     REDR HIT  BLUEL");
		}


	}
}
