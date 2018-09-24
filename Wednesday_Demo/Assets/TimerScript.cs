using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {

	Image timerBar;
	public float maxTime = 5f;
	float timeLeft;


	// Use this for initialization
	void Start () {

		timerBar = GetComponent<Image> ();
		timeLeft = maxTime;
	}
	
	// Update is called once per frame
	void Update () {
		if (timeLeft > 0) {
			timeLeft -= Time.deltaTime;
			timerBar.fillAmount = timeLeft / maxTime;
		} else 
		{
			//timesUpText.SetActive (true);
			Time.timeScale = 0;
		}
		
	}
}
