using UnityEngine;
using System.Collections;

public class MasterChat : MonoBehaviour
{
	class ChatEntry
	{
		public string name = "";
		public string message = "";
		public string timeTag = "";
	}
	
	ArrayList entries;
	Vector2 currentScrollPos = new Vector2 ();
	string inputField = "";
	bool chatInFocus = false;
	string inputFieldFocus = "user";
	public int chat_id = 0;
	
	void Awake ()
	{
		InitializeChat ();
		chat_id = Random.Range (0, 100);
	}
	
	void InitializeChat ()
	{
		entries = new ArrayList ();
		unfocusChat ();
	}
	
	//draw the chat box in size relative to your GUIlayout
	public void Draw ()
	{
		ChatWindow ();
	}
	
	void ChatWindow ()
	{
		GUILayout.BeginVertical ();
		currentScrollPos = GUILayout.BeginScrollView (currentScrollPos, GUILayout.MaxWidth (Screen.width / 4), GUILayout.MinWidth (Screen.width / 8)); //limits the chat window size to max 1000x1000, remove the restraints if you want
		
		foreach (ChatEntry ent in entries) {
			if (ent != null) {
				GUILayout.BeginHorizontal ();
				GUI.skin.label.wordWrap = true;
				GUILayout.Label (ent.timeTag + " " + ent.name + ": " + ent.message);
				GUILayout.EndHorizontal ();
				GUILayout.Space (3);
			}
		}
		
		GUILayout.EndScrollView ();
		if (chatInFocus) {
			GUI.SetNextControlName (inputFieldFocus);
			inputField = GUILayout.TextField (inputField, GUILayout.MaxWidth (1000), GUILayout.MinWidth (1000));
			GUI.FocusControl (inputFieldFocus);
		}
		GUILayout.EndVertical ();
		
		if (chatInFocus) {
			HandleNewEntries ();
		} else {
			checkForInput ();
		}
		
	}
	
	void unfocusChat ()
	{
		//Debug.Log("unfocusing chat");
		inputField = "";
		chatInFocus = false;
	}
	
	void checkForInput ()
	{
		if (Event.current.type == EventType.KeyDown && Event.current.character == '\n' && !chatInFocus) {
			GUI.FocusControl (inputFieldFocus);
			chatInFocus = true;
			currentScrollPos.y = float.PositiveInfinity;
		}
	}
	
	void HandleNewEntries ()
	{
		if (Event.current.type == EventType.KeyDown && Event.current.character == '\n') {
			if (inputField.Length <= 0) {
				unfocusChat ();
				Debug.Log ("unfocusing chat (empty entry)");
				return;
			}
			if (this.GetComponent<MasterNetworking> ().networked) 
				networkView.RPC ("AddChatEntry", RPCMode.All, "Random" + chat_id, inputField);
			else 
				AddChatEntry ("Random" + chat_id, inputField);
			unfocusChat ();
			//Debug.Log("unfocusing chat and entry sent");
		}
	}
	
	[RPC]
	void AddChatEntry (string name, string msg)
	{
		ChatEntry newEntry = new ChatEntry ();
		newEntry.name = name;
		newEntry.message = msg;
		newEntry.timeTag = "[" + System.DateTime.Now.Hour.ToString () + ":" + System.DateTime.Now.Minute.ToString () + "]";
		entries.Add (newEntry);
		currentScrollPos.y = float.PositiveInfinity;
	}
}
