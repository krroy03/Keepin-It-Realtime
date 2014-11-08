using UnityEngine;
using System.Collections;

public class JetControl : MonoBehaviour
{

		private Transform jet;
		private float upperBound;
		private float lowerBound;
		public float speed = 1.0f;
	private float rotationSpeed = 10.0f;
		private bool rotated = false;
		// Use this for initialization
		void Start ()
		{
				jet = this.transform;
				upperBound = 6.0f;
				lowerBound = -7.5f;	
				rotated = false;
		print (jet.eulerAngles);
		}
	
		// Update is called once per frame
		void Update ()
		{	

				if (Input.GetKey ("up")) {
						// move position of jet up
						if (jet.position.y < upperBound) {
								jet.position = new Vector3 (jet.position.x, jet.position.y + Time.deltaTime * speed * rotationSpeed, jet.position.z);
								if (!rotated) {
					jet.eulerAngles = new Vector3(0, 180.0f, -10.0f);
										rotated = true;
								}

								Debug.Log ("Pressed Up");
						}
				} else if (Input.GetKey ("down")) {
						if (jet.position.y > lowerBound) {
				jet.position = new Vector3 (jet.position.x, jet.position.y - Time.deltaTime * speed*rotationSpeed, jet.position.z);
								Debug.Log ("Pressed Down");
								if (!rotated) {
					jet.eulerAngles = new Vector3(0, 180.0f, 10.0f);
										rotated = true;
								}
						}
				} 

				// keep moving jet to the right. 
				jet.position = new Vector3 (jet.position.x + speed, jet.position.y, jet.position.z);
		if (jet.rotation.z != 0 && !Input.GetKey ("up") && !Input.GetKey ("down")) { 
			jet.eulerAngles = new Vector3(0, 180, 0);
						rotated = false;
				}
		}
}
