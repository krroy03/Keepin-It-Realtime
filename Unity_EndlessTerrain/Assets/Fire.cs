using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {

	public Rigidbody rocket;				// Prefab of the rocket.
	public float bulletx = 0.0f;
	public float bullety = 0.0f;
	
	
	void Awake()
	{
	}
	
	
	void Update ()
	{
		// If the fire button is pressed...
		if (Input.GetKeyDown ("space"))
		{
			
			Vector3 bulletPosition = transform.position + new Vector3(bulletx, bullety,0);
				// ... instantiate the rocket facing right and set it's velocity to the right. 
				Rigidbody bulletInstance = Instantiate(rocket, bulletPosition , Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody;
				//bulletInstance.velocity = new Vector3(speed, 0, 0);
			
		}
	}
}
