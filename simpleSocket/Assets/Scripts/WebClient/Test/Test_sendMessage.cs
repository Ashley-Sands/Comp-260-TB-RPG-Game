using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Test_sendMessage : MonoBehaviour
{

	[SerializeField] private TMP_InputField textInput;

	public void SendMessageToServer()
	{

		SocketClient.ActiveSocket.SetMessage( textInput.text );

		textInput.text = "";

	}

}
