using UnityEngine;
using System.Collections;

public class Respawn : MonoBehaviour
{

		public JetControl control;
		public MasterNetworking network;

		private bool dead = false;
		// Use this for initialization
		void Start ()
		{
			
		}
	
		// Update is called once per frame
		void Update ()
		{		
				if (!dead && network.networked) {
						if (network.camScript.outOfScreen || control.health <= 0.0f) {
								dead = true;

								Network.Destroy (control.gameObject);


		
						}
				}
		}

		void OnGUI ()
		{
				if (network.networked && dead) {
						if (GUI.Button (new Rect (0, Screen.height / 5, Screen.width / 4, Screen.height / 8), "Respawn Player")) {

								network.camScript.outOfScreen = false;
								network.SpawnPlayer ();
								dead = false;
						}
				}

		}
}
