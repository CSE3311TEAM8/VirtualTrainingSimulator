using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePrefab : MonoBehaviour {

	public GameObject prefab = null;

	private void Update()
	{
		if (Input.GetKeyDown (KeyCode.Space)) 
		{
			Network.Instantiate (prefab, Vector3.zero, Quaternion.identity, 0);
		}
	}
}
