using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LobbyManager : MonoBehaviour {

	Canvas canvas;
	public MasterNetworking network; 
	
	void Start()
	{
		canvas = GetComponent<Canvas>();

	}
	
	void Update()
	{

	}

	public void HostGame() {
		network.StartServer ();
	}

	public void RefreshGameList() {
		network.RefreshHostList ();

		}


}
