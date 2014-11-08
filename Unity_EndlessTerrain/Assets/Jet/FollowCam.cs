using UnityEngine;
using System.Collections;

public class FollowCam : MonoBehaviour
{

		public GameObject target;
		// Use this for initialization
		void Start ()
		{
				Camera.main.transform.position = new Vector3 (target.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
		}
	
		// Update is called once per frame
		void Update ()
		{
				Camera.main.transform.position = new Vector3 (target.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
		}
}
