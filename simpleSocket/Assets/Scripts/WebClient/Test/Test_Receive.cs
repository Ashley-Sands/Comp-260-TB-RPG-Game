using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Receive : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI textOutput;

	private void Awake ()
	{
		Protocol.HandleProtocol.Inst.Bind('m', ReceiveMessage );
	}

	private void ReceiveMessage( Protocol.BaseProtocol protocol )
	{
		Protocol.MessageProtocol message = protocol as Protocol.MessageProtocol;

		string text = textOutput.text.ToString();

		textOutput.text = message.message + "\n" + text;

		Debug.LogFormat( "Recived message: {0} Len: {1}", message.message, message.message.Length );

	}

	private void OnDestroy ()
	{
		Protocol.HandleProtocol.Inst.Unbind('m', ReceiveMessage );
	}

}
