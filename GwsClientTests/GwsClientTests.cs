using System;
using GwsClient;
using NUnit.Framework;
using Moq;
using SocketIOClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace GwsClient.Tests 
{
	[TestFixture]
	public class GwsClientTests
	{
		private Mock<IClient> _mockClient = null;
		
		[SetUp]
		public void SetUp()
		{
			_mockClient = new Mock<IClient>();
		}
		
		[Test]
		public void SendMessage_EmptyMessage_DoesNotCallSend()
		{
			// Arrange 
			var client = new GwsClient (_mockClient.Object);
			
			// Act
			client.SendMessage(String.Empty, null);

			// Assert
			_mockClient.Verify(c => c.Send(It.IsAny<string>()), Times.Never());
		}
		
		[Test]
		public void SendMessage_ValidMessageName_CallsSend()
		{
			// Arrange 
			var client = new GwsClient (_mockClient.Object);
			var messageName = "testMessage";			
			
			// Act
			client.SendMessage(messageName, null);

			// Assert
			_mockClient.Verify(c => c.Send(It.IsAny<string>()), Times.Once());
		}
		
		[Test]
		public void SendMessage_ValidArguments_CallsSendWithMessageName()
		{
			// Arrange 
			var client = new GwsClient (_mockClient.Object);
			var messageName = "testMessage";			
			
			// Act
			client.SendMessage(messageName, null);

			// Assert
			//string regex = verifyJSONValue("messageName", "testMessage");
			_mockClient.Verify(c => c.Send(It.IsRegex(@".*{\s*""messageName""\s*:\s*""testMessage"".*")), Times.Once());
		}
		
		
		// Utility Methods
		string JsonRegex (string name, string value)
		{
			return String.Format(@".*{\s*""{0}""\s*:\s*""{1}"".*", name, value);
		}
	}
}



