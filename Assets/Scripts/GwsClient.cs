using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIOClient;

public interface IGwsClient 
{
	void SendMessage();
}


public class GwsClient : IGwsClient
{
	private IClient _client;

	public GwsClient (IClient client)
	{
		_client = client;
	}
	
	public void SendMessage ()
	{

	}
}
