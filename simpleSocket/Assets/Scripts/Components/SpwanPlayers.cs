using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class SpwanPlayers : MonoBehaviour
{
	[SerializeField] private Transform[] spwanPoints;
	[SerializeField] private MoveAgent playerPrefab;

	private void Start ()
	{

		int playerId = SocketClient.ActiveGameData.playerID;

		// spwan the local player
		SpwanPlayer( playerId );

		// tell the sever we have joined
		// and bind onto player joined game to await the final list of players
		// so we can begin the game.
		JoinedGameProtocol joinedGame = new JoinedGameProtocol() { player_id = playerId };

		SocketClient.ActiveSocket.QueueMessage( joinedGame );

	}

	void SpwanPlayer( int playerId )
	{

		Protocol.MessageProtocol message = new Protocol.MessageProtocol( "Spwaning @ " + playerId ) { from_client = GameData.GAME_CLIENT_NAME };
		SocketClient.ActiveSocket.QueueLocalMessage( message );

		MoveAgent player = Instantiate<MoveAgent>( playerPrefab, spwanPoints[ playerId ].position, Quaternion.identity );
		player.PlayerId = playerId;

	}

}
