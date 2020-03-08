using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

class DisplayAvailableMatches : MonoBehaviour
{

	private void Awake ()
	{

		HandleProtocol.Inst.Bind( 'g', SetAvailableGames );		

	}

	private void SetAvailableGames( BaseProtocol protocol )
	{

		GameRequestProtocol gamesRequest = protocol as GameRequestProtocol;

		print( "Display match..." + gamesRequest.GetJson(out int _) ); ;

	}

	private void OnDestroy ()
	{

		HandleProtocol.Inst.Unbind( 'g', SetAvailableGames );

	}

}
