using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

	public GameObject target;
	private GameObject[] jets;
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

		Camera.main.transform.position = new Vector3 (target.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
	}
}
