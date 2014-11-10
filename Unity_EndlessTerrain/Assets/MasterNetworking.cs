using UnityEngine;
using System.Collections;

public class MasterNetworking : MonoBehaviour
{


		private const string typeName = "KeepingItRealTime";
		private const string gameName = "Roy";
		public bool networked = false;
		public bool visitor = false;
		private bool showHosts = false;
		public GameObject playerPrefab;
		private Vector3 leaderPos;
		public FollowCam camScript;
		// Use this for initialization
		void Start ()
		{
				networked = false;
				visitor = false;
				showHosts = false;
				leaderPos = Vector3.zero;
		}

		private bool spawned = false;
		// Update is called once per frame
		void Update ()
		{
				if (visitor && !spawned && leaderPos.x > 0) {
						SpawnPlayer ();
						spawned = true;
						Debug.Log ("spawned client at leader pos");
				}

		}

		void OnGUI ()
		{
				this.GetComponent<MasterChat> ().Draw ();

				GUI.Label (new Rect (Screen.width * 3 / 4, 0, Screen.width / 4, Screen.height / 8), "Press enter to chat");

				if (!Network.isClient && !Network.isServer) {
						if (GUI.Button (new Rect (0, Screen.height / 5, Screen.width / 4, Screen.height / 8), "Host your own game!"))
								StartServer ();
			
						if (GUI.Button (new Rect (0, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Show/Hide Host List")) {
								RefreshHostList ();
								showHosts = !showHosts;
						}
						if (showHosts) {
								if (hostList != null) {
										for (int i = 0; i < hostList.Length; i++) {
												if (GUI.Button (new Rect (Screen.width / 4, Screen.height / 8 + (Screen.height / 4 * i), Screen.width / 4, Screen.height / 8), hostList [i].gameName + this.GetComponent<MasterChat> ().chat_id))
														JoinServer (hostList [i]);
										}
								}
						}
				}
		
				if (visitor) {
						if (GUI.Button (new Rect (Screen.width * 3 / 4, Screen.height * 3 / 5, Screen.width / 4, Screen.height / 8), "Leave Server")) {
								Network.Disconnect ();
						} 
			
				} 
		}

		private void StartServer ()
		{
				int port = Random.Range (20000, 22000);
				Network.InitializeServer (4, port, false);
				MasterServer.RegisterHost (typeName, gameName);
	
		}

		void OnServerInitialized ()
		{
				Debug.Log ("Server Initializied");
				networked = true;
				SpawnPlayer ();
		}

		private HostData[] hostList;
	
		private void RefreshHostList ()
		{
				MasterServer.RequestHostList (typeName);
		}
	
		void OnMasterServerEvent (MasterServerEvent msEvent)
		{
				if (msEvent == MasterServerEvent.HostListReceived)
						hostList = MasterServer.PollHostList ();
		}

		private void JoinServer (HostData hostData)
		{
				Network.Connect (hostData);
		}
	
		void OnConnectedToServer ()
		{
				Debug.Log ("Server Joined");
				visitor = true;
				networked = true;
		}

		void OnDisconnectedFromServer (NetworkDisconnection info)
		{
				Debug.Log ("Disconnected from server");
				visitor = false;
				networked = false;
		}

		void OnFailedToConnect (NetworkConnectionError error)
		{
				Debug.Log ("Could not connect to server: " + error);
		}
	
		void OnPlayerConnected ()
		{
		
				Debug.Log ("player Connected, should have spawned player");
				networkView.RPC ("sendLeaderPos", RPCMode.OthersBuffered, camScript.target.transform.position);
		}
	
		void OnPlayerDisconnected ()
		{

				Debug.Log ("A player in the game disconnected...");
	
		}
	
		private void SpawnPlayer ()
		{
				var newPlayer = Network.Instantiate (playerPrefab, leaderPos, playerPrefab.transform.rotation, 0);

		}

		[RPC] 
		void sendLeaderPos (Vector3 pos)
		{
				leaderPos = pos;
				print (pos);
		}
}
