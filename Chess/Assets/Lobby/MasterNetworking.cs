using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif


public class MasterNetworking : MonoBehaviour
{
		// List of players connected to this server
		List<NetworkPlayer> players = new List<NetworkPlayer> ();
		public bool playing;
		public DetectClick dClick;
		private const string typeName = "RoysChessGame";
		private const string gameName = "Grandmaster";
		private bool showStartGame = false;
		private bool playerDisconnected = false;
		public bool visitor = false;
		public bool myTurn = true;
		public bool gotResponse = false;
		public bool gotRequest = false;
		public bool networked = false;
		public SendScore scoreResponse;
		public static MasterNetworking instance;
		// Use this for initialization
		void Awake ()
		{	
		
				if (instance)
						DestroyImmediate (this.gameObject);
				else {
						DontDestroyOnLoad (this.gameObject);
						instance = this;
				}
				scoreResponse = this.GetComponent<SendScore> ();
				showStartGame = false;
				playerDisconnected = false;
				visitor = false;
				myTurn = true;
				gotResponse = false;
				gotRequest = false;
				networked = false;
		
		
		}
	
	
			
		// Update is called once per frame
		void Update ()
		{
				if (networked && Application.loadedLevel == 1) {
			
						dClick = GameObject.FindGameObjectWithTag ("ChessBoard").GetComponent<DetectClick> ();	

				}
		}
	
		private bool showHosts = false;
		private bool surrendered = false;
		private bool draw = false;
		private bool drawRequest = false;
		private bool surrenderRequest = false;
		private bool sentScore = false;
	
		void OnGUI ()
		{	
		
				GUILayout.BeginVertical ();
		
				GUI.Label (new Rect (Screen.width * 3 / 4, 0, Screen.width / 4, Screen.height / 8), "Press enter to chat with opponent or to yourself");
		

				if (!Network.isClient && !Network.isServer) {
						/*
						if (GUI.Button (new Rect (0, Screen.height / 5, Screen.width / 4, Screen.height / 8), "Host your own game!"))
								StartServer ();
			
						if (GUI.Button (new Rect (0, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Show/Hide Host List")) {
								RefreshHostList ();
								showHosts = !showHosts;
						}
						*/
					
						if (hostList != null) {
								for (int i = 0; i < hostList.Length; i++) {
										if (GUI.Button (new Rect (Screen.width * 2 / 4, Screen.height * 4 / 32 + (Screen.height / 16 * i), Screen.width / 10, Screen.height / 24), hostList [i].gameName + this.GetComponent<MasterChat> ().chat_id))
												JoinServer (hostList [i]);
								}
						}

				}
				if (networked && dClick) {
						if (dClick.whiteCheck) {
								GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "White in Check");
						} else if (dClick.blackCheck) {
								GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "Black in Check");
						} else if (dClick.whiteCheckmate) {
								GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "White in Checkmate");
								if (visitor) {
										if (!sentScore) {
												sentScore = true;
												scoreResponse.win = true;
					
										}
										GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Congrats! You Won!");
								} else {
				
										GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Sorry.. You Lost...");
								}
						} else if (dClick.blackCheckmate) {
								GUI.Label (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "Black in Checkmate");
								if (!visitor) {
										if (!sentScore) {
												sentScore = true;
												scoreResponse.win = true;
					
					
										}
										GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Congrats! You Won!");
								} else {
				
										GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Sorry.. You Lost...");
								}
						}
		
						if (networked && playing) {
			
								if (myTurn) {
										GUI.Button (new Rect (0, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Your turn");
								} else {
										GUI.Button (new Rect (0, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Opponent's turn");
								}
			
								if (dClick.gameOver && !visitor) {
										if (surrendered) 
												GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Game Surrendered, You Lost");
										if (surrenderRequest) {
												if (!sentScore) {
														sentScore = true;
														scoreResponse.win = true;
												}
												GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Opponent Surrendered, You Won");
										}				
										if (GUI.Button (new Rect (Screen.width * 2 / 5, Screen.height * 3 / 5, Screen.width / 4, Screen.height / 8), "Start new game")) {
												dClick.resetGame (false);
												networkView.RPC ("sendResetGameNotice", RPCMode.Others);
												draw = false;
												surrendered = false;
												surrenderRequest = false;
												drawRequest = false;
												myTurn = true;
												sentScore = false;
										}
								} else if (dClick.gameOver && visitor) {
										if (surrendered) {
												GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Game Surrendered, You Lost");
										}
										if (surrenderRequest) {
												if (!sentScore) {
														sentScore = true;
														scoreResponse.win = true;
												}
					
												GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Opponent Surrendered, You Won");
										}
										if (GUI.Button (new Rect (Screen.width * 2 / 5, Screen.height * 3 / 5, Screen.width / 4, Screen.height / 8), "Wait for host to start new game or leave if you want"))
												;
				
								}
			
								if (networked) {
										if (GUI.Button (new Rect (Screen.width * 3 / 4, Screen.height * 3 / 5, Screen.width / 4, Screen.height / 8), "Leave Game")) {
												Network.Disconnect ();
										} 
				
								} else if (!visitor && playing) {
										if (GUI.Button (new Rect (Screen.width * 3 / 4, Screen.height * 4 / 5, Screen.width / 4, Screen.height / 8), "Kick opponent out")) {
												Network.CloseConnection (Network.connections [0], true);
												dClick.resetGame (false);
										}
								}
			
								if (GUI.Button (new Rect (Screen.width * 3 / 4, Screen.height / 5, Screen.width / 4, Screen.height / 8), "Surrender Game")) {
										networkView.RPC ("sendSurrenderNotice", RPCMode.Others);
										surrendered = true;
										dClick.gameOver = true;
								} 
								if (GUI.Button (new Rect (Screen.width * 3 / 4, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Request Draw")) {
										networkView.RPC ("sendDrawRequest", RPCMode.Others);
								}
			
								if (drawRequest) {
										if (GUI.Button (new Rect (Screen.width * 2 / 4, Screen.height / 5, Screen.width / 4, Screen.height / 8), "Accept Draw")) {
												networkView.RPC ("sendDrawResponse", RPCMode.Others, true);
												drawRequest = false;
												draw = true;
										}
										if (GUI.Button (new Rect (Screen.width * 2 / 4, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Reject Draw")) {
												networkView.RPC ("sendDrawResponse", RPCMode.Others, false);
												drawRequest = false;
										}
								}
			
								if (draw) {
										dClick.gameOver = true;
				
										GUI.Label (new Rect (Screen.width * 2 / 4, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Game ended in draw!");
				
								}
						}
				}
		
				this.GetComponent<MasterChat> ().Draw ();
		
		
		
				if (showStartGame) {
						if (!visitor) {
								if (GUI.Button (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Player Joined! Start Game"))
										showStartGame = false;
						} else {
								if (GUI.Button (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Joined Game! Host will go first"))
										showStartGame = false;
						}
			
				} else if (!playing && networked)
						GUI.Label (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Waiting For Opponent");
		
				if (playerDisconnected && networked && !visitor) {
						if (GUI.Button (new Rect (Screen.width * 2 / 5, Screen.height * 2 / 5, Screen.width / 4, Screen.height / 8), "Player Disconnected, Waiting for Another player"))
								playerDisconnected = false;
				}
				GUILayout.EndVertical ();
		}
	
		[RPC]
		void sendResetGameNotice ()
		{
				dClick.resetGame (false);
				draw = false;
				surrendered = false;
				surrenderRequest = false;
				drawRequest = false;
				myTurn = false;
				sentScore = false;
		}
	
		[RPC] 
		void sendSurrenderNotice ()
		{
				dClick.gameOver = true;
				surrenderRequest = true;
		}
	
		[RPC] 
		void sendDrawRequest ()
		{
				drawRequest = true;	
		
		}
	
		[RPC] 
		void sendDrawResponse (bool accept)
		{
				draw = accept;
		}
	
		public void StartServer ()
		{		
				Application.LoadLevel ("Chess");


				int port = Random.Range (20000, 22000);
				Network.InitializeServer (2, port, false);
				MasterServer.RegisterHost (typeName, gameName);	
		}
	
		void OnServerInitialized ()
		{
				Debug.Log ("Server Initializied");
				networked = true;
				sentScore = false;
		}
	
		private HostData[] hostList;
	
		public void RefreshHostList ()
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
				Application.LoadLevel ("Chess");
				Network.Connect (hostData);
		}
	
		void OnConnectedToServer ()
		{
				Debug.Log ("Server Joined");
		
				playing = true; 
				visitor = true;
				myTurn = false;
				networked = true;
				showStartGame = true;
				playerDisconnected = false;
		}
	
		void OnDisconnectedFromServer (NetworkDisconnection info)
		{
				Debug.Log ("Disconnected from server");
				playerDisconnected = true;
				visitor = false;
				playing = false;
				networked = false;
				myTurn = true;
		
				showStartGame = false;
				gotResponse = false;
				gotRequest = false;
				showHosts = false;
				surrendered = false;
				draw = false;
				drawRequest = false;
				surrenderRequest = false;
				sentScore = false;
				dClick.resetGame (true);
				Application.LoadLevel ("Lobby");
		}
	
		void OnFailedToConnect (NetworkConnectionError error)
		{
				Debug.Log ("Could not connect to server: " + error);
		}
	
		public void validateMove ()
		{
				networkView.RPC ("validateMoveHelper", RPCMode.Server, dClick.curPiece.row, dClick.curPiece.col, dClick.row, dClick.col);
		}
	
		[RPC]
		void validateMoveHelper (int oldRow, int oldCol, int newRow, int newCol)
		{
				Debug.Log ("host validating move");
				foreach (MoveChessPiece piece in dClick.blackPieces) {
						if (piece.row == oldRow && piece.col == oldCol) 
								dClick.curPiece = piece; 
				}
				dClick.row = newRow;
				dClick.col = newCol;
		
		
				if (dClick.grid [newRow, newCol].hasChessPiece) 
						dClick.attacking = true;
				foreach (MoveChessPiece piece in dClick.whitePieces) {
						if (piece.row == newRow && piece.col == newCol) {
								dClick.attackedPiece = piece;
						}
				}
				gotRequest = true;
		}
	
		public void sendMove (bool valid, bool hostMove)
		{ // server neeeds to send move back to client so client can update its game state
		
				networkView.RPC ("sendMoveHelper", RPCMode.Others, dClick.curPiece.row, dClick.curPiece.col, dClick.row, dClick.col, valid, hostMove);
		
		}
	
		[RPC]
		void sendMoveHelper (int oldRow, int oldCol, int newRow, int newCol, bool valid, bool hostMove)
		{		
		
				Debug.Log ("sending moves from host");
				if (hostMove) {
						var pieces = dClick.whitePieces;
						if (myTurn) // always sending to visitor. If visitor's turn, pieces should be black, else white
								pieces = dClick.blackPieces;
						foreach (MoveChessPiece piece in pieces) {
								if (piece.row == oldRow && piece.col == oldCol) 
										dClick.curPiece = piece; 
						}
						dClick.row = newRow;
						dClick.col = newCol;
			
			
						if (dClick.grid [newRow, newCol].hasChessPiece) 
								dClick.attacking = true;
						foreach (MoveChessPiece piece in dClick.whitePieces) {
								if (piece.row == newRow && piece.col == newCol) {
										dClick.attackedPiece = piece;
								}
						}
				}
				gotResponse = true;
				dClick.moveValidated = valid;
		
		}
	
		void OnPlayerConnected (NetworkPlayer player)
		{
				players.Add (player);
				networkView.RPC ("placeInLine", player, players.Count);
				Debug.Log ("player joined");
				showStartGame = true;
				playing = true;
				playerDisconnected = false;
		}
	
		[RPC]
		void placeInLine (int i)
		{
				Debug.Log ("The server shouldn't get this message");	
		}
	
		void OnPlayerDisconnected (NetworkPlayer player)
		{
				//check the queue for disconnections
				for (int i=0; i<players.Count; i++) {
						if (players [i] == player) {
								Debug.Log ("A player in the waiting queue disconnected...");
								players.RemoveAt (i);
				
						}
				}
		
				if (playing) {
						Debug.Log ("A player in the game disconnected...");
				}
		
				playerDisconnected = true;
				playing = false;
				dClick.resetGame (false);
				myTurn = true;
		}
	
	
}
