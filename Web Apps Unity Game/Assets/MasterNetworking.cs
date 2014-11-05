using UnityEngine;
using System.Collections;

public class MasterNetworking : MonoBehaviour {


	private const string typeName = "KeepingItRealTime";
	private const string gameName = "Roy";
	public bool networked = false;

	
	// Use this for initialization
	void Start () {
		networked = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	private void StartServer()
	{
		Network.InitializeServer(4, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		networked = true;
	}

	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
	}



	private HostData[] hostList;
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}
	
	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}
	
	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
	}

	void OnGUI()
	{	this.GetComponent<MasterChat> ().Draw ();
		if (!Network.isClient && !Network.isServer)
		{
			if (GUI.Button(new Rect(100, 100, 250, 100), "Start Server"))
				StartServer();
			
			if (GUI.Button(new Rect(100, 250, 250, 100), "Refresh Hosts"))
				RefreshHostList();
			
			if (hostList != null)
			{
				for (int i = 0; i < hostList.Length; i++)
				{
					if (GUI.Button(new Rect(400, 100 + (110 * i), 300, 100), hostList[i].gameName))
						JoinServer(hostList[i]);
				}
			}
		}
	}

	void onPlayerConnected() {


		}
}
