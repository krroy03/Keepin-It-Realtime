using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

		public GameObject target;
		private GameObject[] jets;
		public bool outOfScreen;
		public GameObject myJet; 
		// Use this for initialization
		void Start ()
		{
				Camera.main.transform.position = new Vector3 (target.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
		}
	
		// Update is called once per frame
		void Update ()
		{


				var jets = GameObject.FindGameObjectsWithTag ("Player");
				// find jet in the front 
				GameObject inFront = null;
				for (int i = 0; i < jets.Length; i++) {
						var jet = jets [i];
						if (inFront == null) {
								inFront = jet;
						} else {
								if (jet.transform.position.x > inFront.transform.position.x) {
										inFront = jet;
								}
						}
				}


				if (inFront != null) 
						target = inFront;
				if (target != null) 
						Camera.main.transform.position = new Vector3 (target.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
				else
						Camera.main.transform.position = new Vector3 (0.0f, Camera.main.transform.position.y, Camera.main.transform.position.z);	
				// if my jet is behind the leaders jet by more than a certain distance, we destroy it
				if (myJet != null) {
						if (target.transform.position.x > myJet.transform.position.x + 26.0f) {
								outOfScreen = true;
						} else {
								outOfScreen = false;
						}
				}
		}
}
