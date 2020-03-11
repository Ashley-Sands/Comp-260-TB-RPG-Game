using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpwanPlayers : MonoBehaviour
{
	[SerializeField] private Transform[] spwanPoints;
	[SerializeField] private MoveAgent playerPrefab;

	private void Awake ()
	{

		// spwan the local player
		SpwanPlayer( SocketClient.ActiveGameData.currentPlayerID );

		// bind onto player joined game.


	}

	void SpwanPlayer( int playerId )
	{

		MoveAgent player = Instantiate<MoveAgent>( playerPrefab, spwanPoints[ playerId ].position, Quaternion.identity );
		player.PlayerId = playerId;

	}

}
