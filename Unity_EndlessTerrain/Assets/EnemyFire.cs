using UnityEngine;
using System.Collections;

public class EnemyFire : MonoBehaviour {

	public Rigidbody rocket;				// Prefab of the rocket.
	public float bulletx = 0.0f;
	public float bullety = 0.0f;
	private float time = 0.0f;
	
	void Awake()
	{

	}
	
	
	void Update ()
	{	
		time += Time.deltaTime;
		// If the fire button is pressed...
		if (time > 2.0f)
		{
			time=0.0f;
			Vector3 bulletPosition = transform.position + new Vector3(bulletx, bullety,0);
			// ... instantiate the rocket facing right and set it's velocity to the right. 
			Rigidbody bulletInstance = Instantiate(rocket, bulletPosition , Quaternion.Euler(new Vector3(0,0,0))) as Rigidbody;
			//bulletInstance.velocity = new Vector3(speed, 0, 0);
			
		}
	}
}
