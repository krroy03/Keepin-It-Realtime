using UnityEngine;
using System.Collections;

public class pickup : MonoBehaviour {

	void OnTriggerEnter(Collider item)
	{
		if (item.tag == "Player") {
				//Assume only one terrain displayer at a time
				var terrainDisplayer = GameObject.FindObjectOfType (typeof(TerrainDisplayer)) as TerrainDisplayer;        
				if (terrainDisplayer != null && terrainDisplayer.PrefabManager != null && terrainDisplayer.PrefabManager.Pool != null) {
						terrainDisplayer.PrefabManager.Pool.Remove (this.gameObject);
				}				
		}
	}
}
