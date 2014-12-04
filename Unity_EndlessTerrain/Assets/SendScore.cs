﻿using UnityEngine;
using System.Collections;
using SimpleJSON;


public class SendScore : MonoBehaviour {

	//The URL to the server - In our case localhost with port number 1337
	private string url = "http://104.131.44.242/";

	private int score = 0;
	private JetControl control;
	private int userID; 
	private int count = 0;

	// Use this for initialization
	void Start () {
		score = 0;
		control = this.GetComponent<JetControl> ();
		userID = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if (networkView.isMine) {
						count++;
						score = control.score;
			if (count % 600 == 0 && userID >= 0) {
				// so every 10 seconds, update score in database 
				//setup url to the web page that is called
				string customUrl = url + "score/create_or_update";
				
				//setup a form
				WWWForm form = new WWWForm();
				
				//Setup the paramaters
				form.AddField("UserID", userID.ToString());
				form.AddField("Score", score.ToString());
				form.AddField ("GameID", "Platformer");
				
				//Call the server
				WWW www = new WWW(customUrl, form);
				StartCoroutine(WaitForEmptyRequest(www));
			}

			if (userID < 0) {
				Debug.Log ("gets here");
				// get userID if we don't have it yet
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

	}

	void OnGUI()
	{

		
	}

	IEnumerator WaitForEmptyRequest(WWW www)
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

	IEnumerator WaitForRequest(WWW www)
	{
		yield return www;
		
		// check for errors
		if (www.error == null)
		{
			//write data returned
			int temp = -1;
			var keys = JSON.Parse(www.text);

			bool worked = int.TryParse( keys["user"], out temp);
		

			if (worked) {
				userID = temp;
				print (userID);
			}


		} else {
			Debug.Log("WWW Error: "+ www.error);
		}
	}


}
