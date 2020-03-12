﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu( fileName = "GameData", menuName = "GameData" )]
public class GameData : ScriptableObject
{

    private bool inited = false;

    public enum GameStatus { None, Joining, Active }
    public const string GAME_CLIENT_NAME = "GAME";  // this is used as the 'from_client' (in BaseProtocol) when the game needs to inject inbound messages to queue.

    public delegate void connectionStatusChanged ( ConnectionStatus status );
    public event connectionStatusChanged ConnectionStatusChanged;

    public delegate void gameStatusChanged ( GameStatus status );
    public event gameStatusChanged GameStatusChanged;

    public event System.Action GameInfoUpdated;
    public event System.Action< Dictionary<int, string> > PlayersJoined;
    public event System.Action<bool> GameActiveStateChanged;

    // Player Info
    public string nickname = "player";
    public int playerID = 0;            // the Id of the play when in game. this is assigned when the game is launched

    // Game Server Info
    public string gameName = "";
    public List<string> currentLobbyClients = new List<string>();   // TODO: move into current players. i would do it now but theres a lot of work to do on both sides to bing it inline. and i just want ot get it all workinbg atm
    public Dictionary<int, string> currentGamePlayers = new Dictionary<int, string>();
    public int maxPlayers = 4;                  // 4 by default can vary 
    public float gameStartsAt = 0;

    // In Game info
    public bool gameActive = false;
    public int currentPlayerID = 0;         // it would be better if the currentPlayers what a dict with playerId as the key and name as the value. \n
    public string currentPlayerName = "";   // then we can just use the current player id :)

    // Connection and game status
    private ConnectionStatus connStatus = ConnectionStatus.None;
    private GameStatus gameStatus = GameStatus.None;


    public bool Connecting => connStatus == ConnectionStatus.Connecting;
    public bool Connected => connStatus == ConnectionStatus.Connected;
    public bool ConnectionError => connStatus == ConnectionStatus.Error;

    public bool JoingGame => gameStatus == GameStatus.Joining;
    public bool GameActive => gameStatus == GameStatus.Active;

    //private void OnEnable ()
    public void Init()
    {

        if ( inited ) return;   // make sure that this doent happen twice

        connStatus = ConnectionStatus.None; // this is what i hate about scriptables ffs!
        gameStatus = GameStatus.None;

        Protocol.HandleProtocol.Inst.Bind( 'i', ReciveClientIdentityRequest );
        Protocol.HandleProtocol.Inst.Bind( 's', ReceiveServerStatus );
        Protocol.HandleProtocol.Inst.Bind( 'd', ReceiveGameInfo );
        Protocol.HandleProtocol.Inst.Bind( 's', ReceiveOtherClientStatus );     // this is sent from the server when a client joins the game.
        Protocol.HandleProtocol.Inst.Bind( 'b', LaunchGame );                   // TODO: this is not the ideal place for this but hey. i need to make a level manager :|

        Protocol.HandleProtocol.Inst.Bind( 'P', PreStartGame );
        Protocol.HandleProtocol.Inst.Bind( 'S', StartGame );
        Protocol.HandleProtocol.Inst.Bind( 'C', ChangePlayer );

        inited = true;

    }

    private void ReciveClientIdentityRequest ( Protocol.BaseProtocol protocol )
    {

        // fill in the info and send it back to the sever
        Protocol.ClientIdentity clientIdentity = protocol as Protocol.ClientIdentity;

        clientIdentity.nickname = nickname;

        SocketClient.ActiveSocket.QueueMessage( clientIdentity );

    }

    private void ReceiveOtherClientStatus ( Protocol.BaseProtocol protocol )
    {

        currentLobbyClients.Add( protocol.from_client );

        GameInfoUpdated?.Invoke();

    }

    private void ReceiveGameInfo( Protocol.BaseProtocol protocol )
    {
        Protocol.GameInfoProtocol gameInfo = protocol as Protocol.GameInfoProtocol;

        gameName = gameInfo.game_name;
        maxPlayers = gameInfo.max_players;
        gameStartsAt = Time.time + gameInfo.starts_in;
        currentLobbyClients.Clear();
        currentLobbyClients.AddRange( gameInfo.players );

        GameInfoUpdated?.Invoke();

    }

    private void LaunchGame( Protocol.BaseProtocol protocol )
    {
        Protocol.LaunchGameProtocol lGame = protocol as Protocol.LaunchGameProtocol;

        currentGamePlayers.Clear(); // starting a new game

        playerID = lGame.player_id;

        currentGamePlayers.Add( playerID, nickname );


        SceneManager.LoadScene( "SampleScene", LoadSceneMode.Single );

    }

    private void PreStartGame( Protocol.BaseProtocol protocol )
    {

        Protocol.PreStartGameProtocol preStartGame = protocol as Protocol.PreStartGameProtocol;
        currentGamePlayers.Clear();

        for ( int i = 0; i < preStartGame.player_ids.Length; i++ )
        {
            int pid = preStartGame.player_ids[ i ];
            string pname = preStartGame.player_names[ i ];

            currentGamePlayers.Add( pid, pname );
        }

        currentPlayerName = currentGamePlayers[ 0 ];
        PlayersJoined?.Invoke( currentGamePlayers );

    }

    private void StartGame( Protocol.BaseProtocol protocol )
    {
        Protocol.StartGameProtocol startgame = protocol as Protocol.StartGameProtocol;

        if ( startgame.ok )
            gameActive = true;
        // else
        // about the game
        // TODO: above      

        GameInfoUpdated?.Invoke();
        GameActiveStateChanged?.Invoke( gameActive );

    }

    private void ChangePlayer( Protocol.BaseProtocol protocol )
    {

        Protocol.ChangePlayerProtocol changePlayer = protocol as Protocol.ChangePlayerProtocol;

        currentPlayerID = changePlayer.player_id;
        currentPlayerName = currentGamePlayers[ currentPlayerID ];

        GameInfoUpdated?.Invoke();

    }

    /// <summary>
    /// are we the current player
    /// </summary>
    /// <param name="pId">id of the player</param>
    /// <returns></returns>
    public bool PlayerIsActive(  )
    {
        return gameActive && currentPlayerID == playerID;
    }

    /// <summary>
    /// are we pid
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public bool IsPlayer( int pid )
    {
        return playerID == pid;
    }

    /// <summary>
    /// are we pid and the current player
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public bool IsPlayerAndActive( int pid )
    {
        return IsPlayer( pid ) && PlayerIsActive();
    }

    private void ReceiveServerStatus( Protocol.BaseProtocol protocol )
    {

        Protocol.StatusProtocol serverStatus = protocol as Protocol.StatusProtocol;

        if ( serverStatus.IsType( Protocol.StatusProtocol.Type.Server ) && !serverStatus.ok )
            SetConnectionStatus( ConnectionStatus.Error );

    }

    public void SetConnectionStatus( ConnectionStatus status )
    {
        if (status != connStatus && connStatus != ConnectionStatus.Error)
        {
            Debug.Log( "Set connection status " + status );

            ConnectionStatusChanged?.Invoke( status );
            connStatus = status;

        }

        // TODO: reset game status??

    }

    public void SetGameStatus( GameStatus status )
    {
        
        if (status != gameStatus)
        {
            Debug.Log( "Set game status " + status );
            gameStatus = status;
            GameStatusChanged?.Invoke( status );
        }

    }

    public void ResetConnectionStatus()
    {
        connStatus = ConnectionStatus.None;
        ConnectionStatusChanged?.Invoke( connStatus = ConnectionStatus.None );

        // TODO: reset game status with connection??

    }

    private void OnDestroy ()
    {
        Protocol.HandleProtocol.Inst.Unbind( 'i', ReciveClientIdentityRequest );
        Protocol.HandleProtocol.Inst.Unbind( 's', ReceiveServerStatus );
        Protocol.HandleProtocol.Inst.Unbind( 'd', ReceiveGameInfo );
        Protocol.HandleProtocol.Inst.Unbind( 's', ReceiveOtherClientStatus );
        Protocol.HandleProtocol.Inst.Unbind( 'P', PreStartGame );
        Protocol.HandleProtocol.Inst.Unbind( 'S', StartGame );
        Protocol.HandleProtocol.Inst.Unbind( 'C', ChangePlayer );


    }


}