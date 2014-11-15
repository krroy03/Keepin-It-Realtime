﻿using UnityEngine;
using System.Collections;

public class SendScore : MonoBehaviour {

	//The URL to the server - In our case localhost with port number 2475
	private string url = "http://localhost:1337/";

	private int score = 0;
	private JetControl control;
	private int userID; 

	// Use this for initialization
	void Start () {
		score = 0;
		control = this.GetComponent<JetControl> ();
		userID = -1;
	}
	
	// Update is called once per frame
	void Update () {
		score = control.score;
	}

	void OnGUI()
	{
		// Make a background box
		GUI.Box(new Rect(10,10,100,90), "Loader Menu");
		
		// Make the first button.
		if(GUI.Button(new Rect(20,40,80,20), "SendScore") && userID >= 0) {
			
			//when the button is clicked
			
			//setup url to the web page that is called
			string customUrl = url;
			
			//setup a form
			WWWForm form = new WWWForm();
			
			//Setup the paramaters
			form.AddField("UserID", userID.ToString());
			form.AddField("Score", score.ToString());
			
			//Call the server
			WWW www = new WWW(customUrl, form);
			StartCoroutine(WaitForRequest(www));
		}

		// make the second button 
		if(GUI.Button(new Rect(20,40,80,20), "GetUserID")) {
			
			//when the button is clicked
			
			//setup url to the webpage that is called
			string customUrl = url + "user/current_user/";
			
			//setup a form
			WWWForm form = new WWWForm();
			
			//Setup the paramaters
			//
			
			//Call the server
			WWW www = new WWW(customUrl, form);
			StartCoroutine(WaitForRequest(www));
		}
		
	}

	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			//write data returned
			Debug.Log(www.text);
			
		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}


}
