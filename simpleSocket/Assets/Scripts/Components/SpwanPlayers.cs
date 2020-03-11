using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPlayers : MonoBehaviour
{
	[SerializeField] private Transform[] spwanPoints;
	[SerializeField] private MoveAgent playerPrefab;

	private void Start ()
	{

		// spwan the local player
		SpwanPlayer( SocketClient.ActiveGameData.playerID );

		// bind onto player joined game.


	}

	void SpwanPlayer( int playerId )
	{

		Protocol.MessageProtocol message = new Protocol.MessageProtocol( "Spwaning @ " + playerId ) { from_client = GameData.GAME_CLIENT_NAME };
		SocketClient.ActiveSocket.QueueLocalMessage( message );

		MoveAgent player = Instantiate<MoveAgent>( playerPrefab, spwanPoints[ playerId ].position, Quaternion.identity );
		player.PlayerId = playerId;

	}

}
