using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public Rigidbody rocket;				// Prefab of the rocket.

	
	
	void Awake()
	{
	}
	
	
	void Update ()
	{
		// If the fire button is pressed...
		if (Input.GetKeyDown ("space"))
		{
			

				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody bulletInstance = Instantiate(rocket, transform.position + new Vector3(3.0f, 0, 0), Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody;
				//bulletInstance.velocity = new Vector3(speed, 0, 0);
			
		}
	}
}
