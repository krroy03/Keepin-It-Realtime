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
		private MasterNetworking network;
		
		// health and score variables
		public float health = 100.0f;
		public float barDisplay; //current progress
		public Vector2 pos = new Vector2 (20, 40);
		public Vector2 size = new Vector2 (60, 20);
		public Texture2D emptyTex;
		public Texture2D fullTex;

		// jet has a score 
		// score increases the further jet gets without being destroyed. 

		public int score = 0;
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
				network = GameObject.FindGameObjectWithTag ("Network").GetComponent<MasterNetworking> ();
				
				fullTex = new Texture2D (60, 20);
				// Fill the texture with Sierpinski's fractal pattern!
				for (int y = 0; y < fullTex.height; ++y) {
						for (int x = 0; x < fullTex.width; ++x) {
								fullTex.SetPixel (x, y, Color.green);
						}
				}
				// Apply all SetPixel calls
				fullTex.Apply ();

		}
	
		// Update is called once per frame
		void Update ()
		{		
				networked = network.networked;
				if (networked && networkView.isMine) {
						inputMovement ();
						// update score 
						if (!collided)
								score += 5;
					
						barDisplay = health / 100.0f;

				} else {
			inputMovement ();
				}
			
				
		}
		
		void OnGUI ()
		{
				// show score for your own character
				
				if (networked && networkView.isMine) {
						GUI.Button (new Rect (0, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "Score: " + score.ToString ());
					
						//	draw health
						//GUI.Box (new Rect (Screen.width *4/5, Screen.height * 4 / 5, size.x, size.y), emptyTex);
			
						//draw the filled-in part:
						GUI.Box (new Rect (Screen.width * 4 / 5, Screen.height * 4 / 5, size.x * barDisplay, size.y), fullTex);

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

		void OnTriggerEnter (Collider col)
		{
				if (col.CompareTag ("Bullet") && health > 0) {
						// if hits missle, lose hp
						health -= 10.0f;
						print (health);
			score -= 500;
				}
		}

		void OnCollisionEnter (Collision col)
		{
				if (networkView.isMine) {
						if (col.collider.CompareTag ("Player")) {
								jet.collider.isTrigger = true;
								// pass through other players
						} else {
								collided = true;
								jetBody.isKinematic = true;
								oldSpeed = jetSpeed;
								jetSpeed = 0.001f;
						}
		
						
				}
		}

		void OnCollisionExit (Collision col)
		{
				if (networkView.isMine) {
						if (col.collider.CompareTag ("Player")) {
								jet.collider.isTrigger = false;
						} else {
								collided = false;
								jet.position = new Vector3 (jet.position.x, jet.position.y, jetDepth);
								jet.eulerAngles = new Vector3 (0.0f, 180.0f, 0.0f);
								jetBody.isKinematic = false;
								jetSpeed = oldSpeed;
								
								
						}
				}
		}
}
