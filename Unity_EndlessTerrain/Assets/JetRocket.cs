using UnityEngine;
using System.Collections;

public class JetRocket : MonoBehaviour {

	public GameObject explosion; // prefab for explosion
	private float startTime;
	public float speed = 20.0f;
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		this.rigidbody.isKinematic = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime > 2.0f) 
			
			Destroy (this.gameObject);

		this.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
	}

	void OnExplode()
	{
		// Create a quaternion with a random rotation in the z-axis.
		Quaternion randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
		
		// Instantiate the explosion where the rocket is with the random rotation.
		Instantiate(explosion, transform.position, randomRotation);
	}

	void OnCollisionEnter(Collision col) {
		if (col.collider.CompareTag ("Enemy")) {
						OnExplode ();
						Destroy (col.gameObject);
						Destroy (this.gameObject);
		} else if (col.collider.CompareTag ("Bullet")) {
			OnExplode ();
			Destroy (col.gameObject);
			Destroy (this.gameObject);
				}
		else if (col.collider.CompareTag("Player")) {
			OnExplode ();
			Destroy (this.gameObject);
		}
	}
	
}
