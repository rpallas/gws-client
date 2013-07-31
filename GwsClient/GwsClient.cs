using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SocketIOClient;

namespace GwsClient
{
	public interface IGwsClient 
	{
		void SendMessage(string messageName, object data);
	}

	public class GwsClient : IGwsClient
	{
		private IClient _client;

		public GwsClient (IClient client)
		{
			_client = client;
		}
		
		public void SendMessage (string messageName, object data)
		{
			if(string.IsNullOrEmpty(messageName))
			{
				return;
			}
			_client.Send (string.Empty);
		}
	}
}
