using UnityEngine;
using System.Collections;

public class JetControl : MonoBehaviour
{

		private Transform jet;
		private float upperBound;
		private float lowerBound;
		public float speed = 1.0f;
		public float rotationSpeed = 10.0f;
		private bool rotated = false;
		private bool collided = false;
		public bool networked = false;
		// Use this for initialization
		void Start ()
		{
				jet = this.transform;
				upperBound = 6.0f;
				lowerBound = -7.5f;	
				rotated = false;
				collided = false;
				networked = false;
		}
	
		// Update is called once per frame
		void Update ()
		{
				if (networked && networkView.isMine) {
						inputMovement ();
				} else if (!networked) {
						inputMovement ();
				}
		}
		
		void inputMovement ()
		{
			// first, make sure that the jet does not go above or below ground and ceiling
			// also, make sure that jet is always 
		
				if (jet.position.y < lowerBound) {
						jet.position = new Vector3 (jet.position.x, lowerBound, jet.position.z);
				} else if (jet.position.y > upperBound) {
						jet.position = new Vector3 (jet.position.x, upperBound, jet.position.z);
				}
		
				if (Input.GetKey ("up")) {
						// move position of jet up
						if (jet.position.y < upperBound) {
								jet.position = new Vector3 (jet.position.x, jet.position.y + Time.deltaTime * speed * rotationSpeed, jet.position.z);
								if (!rotated) {
										jet.eulerAngles = new Vector3 (0, 180.0f, -10.0f);
										rotated = true;
								}
				
								if (collided) {
										collided = false;
								}
								Debug.Log ("Pressed Up");
						}
				} else if (Input.GetKey ("down")) {
						if (jet.position.y > lowerBound) {
								jet.position = new Vector3 (jet.position.x, jet.position.y - Time.deltaTime * speed * rotationSpeed, jet.position.z);
								Debug.Log ("Pressed Down");
								if (!rotated) {
										jet.eulerAngles = new Vector3 (0, 180.0f, 10.0f);
										rotated = true;
								}
								if (collided) {
										collided = false;
								}
						}
				} 
		
				// keep moving jet to the right as long as its not colliding with anything
				jet.position = new Vector3 (jet.position.x + speed, jet.position.y, jet.position.z);
				if (collided) {
						jet.eulerAngles = new Vector3 (0, 180, 0);
				}
				if (jet.rotation.z != 0 && !Input.GetKey ("up") && !Input.GetKey ("down")) { 
						jet.eulerAngles = new Vector3 (0, 180, 0);
						rotated = false;
				}
		
				jet.position = new Vector3 (jet.position.x, jet.position.y, -15.0f);
	
		}

		void OnCollisionEnter (Collision col)
		{
				collided = true;
		}
}
