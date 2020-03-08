using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class Test_Receive : MonoBehaviour
{
	[SerializeField] private TMPro.TextMeshProUGUI textOutput;

	private void Awake ()
	{
		HandleProtocol.Inst.Bind('m', ReceiveMessage );
		HandleProtocol.Inst.Bind( 's', ReceiveClientStatus );
	}

	private void ReceiveMessage( BaseProtocol protocol )
	{
		MessageProtocol message = protocol as MessageProtocol;

		PrintToConsole( string.Format( "{0}: {1}", message.from_client, message.message ) );

		Debug.LogFormat( "Recived message: {0} from {2} Len: {1}", message.message, message.message.Length, message.from_client );

	}

	private void ReceiveClientStatus( BaseProtocol protocol )
	{
		ClientStatusProtocol clientStatus = protocol as ClientStatusProtocol;

		PrintToConsole( string.Format( "{0} has {1} the server", clientStatus.from_client, ( clientStatus.connected ? "connected to" : "disconnected from" ) ) );

	}

	private void PrintToConsole( string message )
	{
		string text = textOutput.text.ToString();

		textOutput.text = string.Format( "{0}\n{1}", message, text );

	}

	private void OnDestroy ()
	{
		HandleProtocol.Inst.Unbind('m', ReceiveMessage );
		HandleProtocol.Inst.Unbind('s', ReceiveClientStatus );
	}

}
