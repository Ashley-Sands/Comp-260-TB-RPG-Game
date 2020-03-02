using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Receive : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI textOutput;

	private void Update ()
	{
		string message = SocketClient.ActiveSocket.GetMessage();
		if (message.Length > 0)
		{
			print( message.Length + " ~~~~ " + message[ message.Length - 1 ] );

			string text = textOutput.text.ToString();
			Debug.Log( "fdsf@@ "+text );
			Debug.LogWarning( "@@@"+ message + text );
			Debug.Log( "###"+text );

			textOutput.text = message +"\n"+ text;

		}

	}

}
