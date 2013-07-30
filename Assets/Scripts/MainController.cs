using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIOClient;

//using pomeloUnityClient;

public class MainController : MonoBehaviour 
{	
	private Client _client;	
	private Player _opponent = null;
	private string _userName = String.Empty;
	private bool _loggedIn = false;
	private int _currentQuestionIndex = 0;
	private List<Question> _questions = new List<Question>();
	
	
	// Use this for initialization
	void Start () 
	{
		GenerateTestQuestions(); // Temporary - test data
		
		_client = new Client("http://gws-server.rpallas_1.c9.io");
		
		_client.Opened += SocketOpened;
		_client.Message += SocketMessage;
		_client.SocketConnectionClosed += SocketConnectionClosed;
		_client.Error += SocketError;
		
		_client.Connect();			
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}	

	/// <summary>
	/// Sends a message to the server.
	/// </summary>
	void SendMessage (string msgName, JSONObject jsonData)
	{
		var data = jsonData != null ? jsonData : JSONObject.nullJO;
		JSONObject msg = new JSONObject(JSONObject.Type.OBJECT);
		msg.AddField("msg", msgName);
		msg.AddField("data", data);
		_client.Send(msg.print());
	}
			
	void OnGUI () 
	{
		//GUISizer.BeginGUI();
		
		var currentQuestion = _questions[_currentQuestionIndex];
		
		string connectionStatus = "Not Connected";
		if (_client != null && _client.IsConnected)		
		{
			connectionStatus = "Connected";
		}
		GUI.Box(new Rect(Screen.width - 110,10,100,20), connectionStatus);

		string versesPlayer = IsMatched() ? _opponent.UserName : "Not Matched";
//		Debug.Log("_opponent: " + versesPlayer);
		GUI.Box(new Rect(10,10,150,20), "Vs: " + versesPlayer);
		
		if(IsMatched() && currentQuestion != null)
		{			
			// Make a group on the center of the screen
			GUI.BeginGroup (new Rect (Screen.width / 2 - 100, 50, 300, 300));
			// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.		
			
				// Make a background box
				GUI.Box(new Rect(0,0,200,200), currentQuestion.QuestionText);		
						
				var answerTop = 20;
				currentQuestion.Answers.ForEach(answer => 
				{
					if(GUI.Button(new Rect(20,answerTop += 30,160,20), answer.AnswerText)) 
					{
						
					}
				});
			
			GUI.EndGroup();
		}
		// Make a group on the center of the screen
		GUI.BeginGroup (new Rect (Screen.width / 2 - 100, 250, 200, 200));
		// All rectangles are now adjusted to the group. (0,0) is the topleft corner of the group.		
			
			if(!_loggedIn)
			{				
				_userName = GUI.TextField(new Rect(20, 15, 160, 20), _userName);
				if(GUI.Button(new Rect(20,50,160,30), "Login")) 
				{					
					var data = new JSONObject(JSONObject.Type.OBJECT);
					data.AddField("userName", _userName);
					SendMessage("login", data);
				
					_loggedIn = true;
				}
			} 		
			else if (!IsMatched()) 
			{
				if(GUI.Button(new Rect(20,50,160,30), "Find Match")) 
				{
					SendMessage("findMatch", JSONObject.nullJO);
				}
			}
			else 
			{
				// Show game controls
				// Quit Button
				if(GUI.Button(new Rect(0,50,60,30), "Quit")) 
				{
					SendMessage("quitMatch", JSONObject.nullJO);
				}
				// Next Button
				if(GUI.Button(new Rect(80,50,60,30), "Next")) 
				{
					SendMessage("nextMatch", JSONObject.nullJO);
					NextQuestion();
				}
			}

		
		GUI.EndGroup();
		
		//GUISizer.EndGUI();
	}
	
	void OnApplicationQuit()
	{
		closeSocketIO();
		_loggedIn = false;
		SendMessage("applicationQuit", JSONObject.nullJO);
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
		
			// If its Json parse it
			if(MessageIsJson(msg))
			{
				var json = new JSONObject(msg);	
				Debug.Log("Json '"+json["msg"]+"' message received: " + json["data"].print());				
				HandleMessage(json["msg"].str, new JSONObject(json["data"].print ()));
			}		
		}	    
	}

	/// <summary>
	/// Handles the message.
	/// </summary>
	/// <param name='message'>
	/// The name of the message 
	/// </param>
	void HandleMessage (string message, JSONObject data)
	{
		switch (message) {
			case "begin":
				_opponent = new Player 
				{ 
					Id = data["opponent"]["id"].str, 
					UserName =  data["opponent"]["userName"].str
				};
				break;
			default:
				Debug.Log("message '" + message + "' not recognised");
				break;
		}
		
	}

	/// <summary>
	/// Changes the current question to the nexts available one 
	/// or resets it to the first
	/// </summary>
	void NextQuestion()
	{
		if (_currentQuestionIndex >= _questions.Count - 1) {
			_currentQuestionIndex = 0;
		} else {
			_currentQuestionIndex++;
		}
	}

	/// <summary>
	/// Checks if the message text is Json
	/// </summary>
	/// <returns>
	/// True if the message text is in Json format.
	/// </returns>
	/// <param name='msg'>
	/// The message text.
	/// </param>
	private bool MessageIsJson (string msg)
	{
		return msg.StartsWith("{");
	}

	/// <summary>
	/// Determines whether this instance is matched.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is matched; otherwise, <c>false</c>.
	/// </returns>

	bool IsMatched ()
	{
		return _opponent != null;
	}
	
//	private void ParseJson(JSONObject obj)
//	{
//		switch(obj.type)
//		{
//			case JSONObject.Type.OBJECT:
//				for(int i = 0; i < obj.list.Count; i++)
//				{
//					string key = (string)obj.keys[i];
//					JSONObject j = (JSONObject)obj.list[i];
//					Debug.Log(key);
//					ParseJson(j);
//				}
//				break;
//			case JSONObject.Type.ARRAY:
//				foreach(JSONObject j in obj.list)
//				{
//					ParseJson(j);
//				}
//				break;
//			case JSONObject.Type.STRING:
//				Debug.Log(obj.str);
//				break;
//			case JSONObject.Type.NUMBER:
//				Debug.Log(obj.n);
//				break;
//			case JSONObject.Type.BOOL:
//				Debug.Log(obj.b);
//				break;
//			case JSONObject.Type.NULL:
//				Debug.Log("NULL");
//				break;
//		}
//	}
	
	private void SocketConnectionClosed (object sender, EventArgs e) 
	{
		closeSocketIO();
		//_client.Send();
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
	
	// Temporary method to generate some sample question data
	void GenerateTestQuestions ()
	{
		_questions = new List<Question>();
			
		int answerId = 0;
		for(int i = 0; i<10; i++)
		{
			var answers = new List<Answer>()
			{
				new Answer(){ Id = answerId++, AnswerText="Correct answer" },
				new Answer(){ Id = answerId++, AnswerText="Wrong answer" }
			};
			var question = new Question() 
			{ 
				Id = 1, 
				QuestionText = "Question " + (i + 1),
				Answers = answers				
			};
			_questions.Add(question);
		}
	}
}
