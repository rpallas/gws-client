using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIOClient;
using SimpleJson;
using SimpleJSON;
//using pomeloUnityClient;

public class MainController : MonoBehaviour 
{	
	private Client _client;	
	//private PomeloClient _client;
	//private string _userId;
	private string _opponent = null;
	private string _userName = "";
	
	// Use this for initialization
	void Start () 
	{
		_client = new Client("http://gws.rpallas_1.c9.io");
		
		_client.Opened += SocketOpened;
		_client.Message += SocketMessage;
		_client.SocketConnectionClosed += SocketConnectionClosed;
		_client.Error += SocketError;
		
		_client.Connect();
		
//		_client = new PomeloClient("http://gws.rpallas_1.c9.io");
//		
//		_client.On("onconnected", (data) => {
//			object id = null;
//			Debug.Log(data.ToString());	
//			if(data.TryGetValue("id", out id))
//			{
//				Debug.Log(id);	
//			}
//		});
//		
//		_client.init();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}	
			
	void OnGUI () 
	{
		string connectionStatus = "Not Connected";
		if (_client != null && _client.IsConnected)		
		{
			connectionStatus = "Connected";
		}
		GUI.Box(new Rect(Screen.width - 110,10,100,20), connectionStatus);

		string versesPlayer = _opponent != null ? _opponent : "Not Matched";
//		Debug.Log("_opponent: " + versesPlayer);
		GUI.Box(new Rect(10,10,150,20), "Vs: " + versesPlayer);
		
		// Make a group on the center of the screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, 50, 300, 300));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.		
		
			// Make a background box
			GUI.Box(new Rect(0,0,200,200), "Question text ...");		
						
			if(GUI.Button(new Rect(20,50,160,20), "Right Answer")) 
			{
				Application.LoadLevel(1);
			}
	
			if(GUI.Button(new Rect(20,80,160,20), "Wrong Answer")) 
			{
				Application.LoadLevel(2);
			}
		
		GUI.EndGroup();
		
		// Make a group on the center of the screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, 250, 200, 200));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.		
			
			_userName = GUI.TextField(new Rect(20, 15, 160, 20), _userName);
		
			if(GUI.Button(new Rect(20,50,160,30), "Find Match")) 
			{
				_client.Send("findMatch");
//				JsonObject userMessage = new JsonObject();
//				userMessage.Add("userName", _userName);
//				_client.notify("findMatch", userMessage);
			}
		
		GUI.EndGroup();
	}
	
	void OnApplicationQuit()
	{
		closeSocketIO();
		//_client.disconnect();
	}

	private void SocketOpened(object sender, EventArgs e) 
	{
		Debug.Log("Socket opened");
	}
	
	private void SocketMessage (object sender, MessageEventArgs e) 
	{
		Debug.Log("Event: " + e.Message.Event);
		if ( e!= null && e.Message.Event == "message") 
		{
			string msg = e.Message.MessageText;
			Debug.Log("Message received: " + msg);
			try
			{
				var json = JSON.Parse(msg);
				Debug.Log("Json '"+json[msg].Value+"' message received: " + json);				
				// Message routing
				switch (json[msg].Value) 
				{
					case "begin":
						_opponent = json["data"]["oppenent"]["id"].Value;
						break;
					default:
					break;
				}
			}
			catch
			{
				Debug.Log("Non json message received: " + msg);
			}
		}	    
	}
	
	private void SocketConnectionClosed (object sender, EventArgs e) 
	{
		closeSocketIO();
	}
	
	private void SocketError (object sender, ErrorEventArgs e) 
	{
		if(sender is Client)
		{			
			var client = sender as Client;
			Debug.LogError("Socket error: " + client.LastErrorMessage);
			if(client.HandShake.HadError)
			{
				Debug.LogError("Handshake error: " + client.HandShake.ErrorMessage);
			}
		} 
		else 
		{
			Debug.LogError("Socket error: " + e.Message);
		}
	}
	
	//Free the resources
	private void closeSocketIO()
	{
		if (_client != null) 
		{			
			_client.Opened -= SocketOpened;
			_client.Message -= SocketMessage;
			_client.SocketConnectionClosed -= SocketConnectionClosed;
			_client.Error -= SocketError;
			_client.Close();
			
			Debug.Log("Socket closed");
			
			_client = null;
			
		}
	}
}
