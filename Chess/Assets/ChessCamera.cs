using UnityEngine;
using System.Collections;

public class ChessCamera : MonoBehaviour
{

		public MasterNetworking server;
		private bool rotated = false;
		// Use this for initialization
		void Awake ()
		{
		
		server = GameObject.FindGameObjectWithTag ("Network").GetComponent<MasterNetworking> ();
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (server.networked) {
						
						if (server.visitor) {
								transform.eulerAngles = new Vector3 (90.0f, 270.0f, 180.0f);
								rotated = true;
						} 
				} else {
						if (rotated) {
								transform.eulerAngles = new Vector3 (90.0f, 270.0f, 0f);
				rotated = false;
			}
				}

		}
}
