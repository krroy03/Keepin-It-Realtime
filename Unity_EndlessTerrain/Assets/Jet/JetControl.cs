using UnityEngine;
using System.Collections;

public class JetControl : MonoBehaviour
{

		private Transform jet;
		private float upperBound;
		private float lowerBound;
		public float jetSpeed = 0.25f;
		private float oldSpeed = 0f;
		public float rotationSpeed = 10.0f;
		private bool rotated = false;
		private bool collided = false;
		public bool networked = false;
		public float jetDepth = -15.0f;
		public Rigidbody jetBody;
		// Use this for initialization
		void Start ()
		{
				jet = this.transform;
				upperBound = 7.5f;
				lowerBound = -7.5f;	
				rotated = false;
				collided = false;
				networked = false;
				jetBody = this.GetComponent<Rigidbody> ();
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
						jet.position = new Vector3 (jet.position.x, lowerBound, jetDepth);
				} else if (jet.position.y > upperBound) {
						jet.position = new Vector3 (jet.position.x, upperBound, jetDepth);
				}
				
				if (Input.GetKey ("up")) {
						// move position of jet up
						if (jet.position.y < upperBound) {
								jet.position = new Vector3 (jet.position.x, jet.position.y + Time.deltaTime * jetSpeed * rotationSpeed, jetDepth);
								if (!rotated) {
										jet.eulerAngles = new Vector3 (0.0f, 180.0f, -10.0f);
										rotated = true;
								}

						}
				} else if (Input.GetKey ("down")) {
						if (jet.position.y > lowerBound) {
								jet.position = new Vector3 (jet.position.x, jet.position.y - Time.deltaTime * jetSpeed * rotationSpeed, jetDepth);
								if (!rotated) {
										jet.eulerAngles = new Vector3 (0.0f, 180.0f, 10.0f);
										rotated = true;
								}
						}
				} 
		
				// keep moving jet to the right as long as its not colliding with anything
				jet.position = new Vector3 (jet.position.x + jetSpeed, jet.position.y, jetDepth);
				
				if (!Input.GetKey ("up") && !Input.GetKey ("down")) { 
						jet.eulerAngles = new Vector3 (0.0f, 180.0f, 0.0f);
						rotated = false;
				}
		
				
	
		}

		void OnCollisionEnter (Collision col)
		{
				if (networkView.isMine) {
						collided = true;
						jetBody.isKinematic = true;
						oldSpeed = jetSpeed;
						jetSpeed = 0.001f;
				}
		}

		void OnCollisionExit (Collision col)
		{
				if (networkView.isMine) {
						collided = false;
						jet.position = new Vector3 (jet.position.x, jet.position.y, jetDepth);
						jet.eulerAngles = new Vector3 (0.0f, 180.0f, 0.0f);
						jetBody.isKinematic = false;
						jetSpeed = oldSpeed;
						print ("exit collision");
				}
		}
}
