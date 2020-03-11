using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test_sendMessage : MonoBehaviour
{

	[SerializeField] private TMP_InputField textInput;

	public void SendMessageToServer()
	{

		Protocol.MessageProtocol message = new Protocol.MessageProtocol( textInput.text );

		SocketClient.ActiveSocket.QueueMessage( message );

		textInput.text = "";

	}

}
