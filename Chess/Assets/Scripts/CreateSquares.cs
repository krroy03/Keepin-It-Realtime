using UnityEngine;
using System.Collections;

public class CreateSquares : MonoBehaviour {
	public GameObject square;

	// Use this for initialization
	void Start () {
		// make 64 squares, 1 for each cell on chess board, and position them over each cell, make them transparent
		// a difference of 1.1 between cells on z axis and x axis
		// y axis always sent to 0.5
		// each square has a rigidbody attached to it
		var diff = 1.2f;
		var topLeft = new Vector3 (-3.5f * diff, 0.0f, -3.5f * diff);
		var cur = square;
		var properties = cur.GetComponent<SquareProperties> ();
		for (int i=0; i < 8; i++) {
			for (int j=0; j < 8; j++) {
				cur = (GameObject) Instantiate (square, new Vector3( topLeft.x + i * diff, topLeft.y, topLeft.z + j * diff), Quaternion.identity);
				properties.row = i;
				properties.col = j;
				properties.label = i*8 + j;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
