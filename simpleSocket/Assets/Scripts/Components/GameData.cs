using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu( fileName = "GameData", menuName = "GameData" )]
public class GameData : ScriptableObject
{

    private bool inited = false;

    public enum GameStatus { None, Joining, Active }
    public const string GAME_CLIENT_NAME = "GAME";  // this is used as the 'from_client' (in BaseProtocol) when the game needs to inject inbound messages to queue.

    public delegate void connectionStatusChanged ( ConnectionStatus status, string message );
    public event connectionStatusChanged ConnectionStatusChanged;

    public delegate void gameStatusChanged ( GameStatus status );
    public event gameStatusChanged GameStatusChanged;

    public event System.Action GameInfoUpdated;
    public event System.Action<Dictionary<int, Client>> PlayersJoined;
    public event System.Action<bool> GameActiveStateChanged;
    public event System.Action<int> PlayerDisconnected;

    // Lobby
    private int nextLobbyId = 0;  // when we're in the lobby the clients do not have player id's so we'll assign this.

    // Player Info
    public LocalClient localClient = new LocalClient("Local Client");           

    // Game Server Info
    public string gameName = "";

    public Dictionary<int, Client> currentGamePlayers = new Dictionary<int, Client>();  // key: player id

    public int minPlayers = 2;                  // 4 by default can vary 
    public int maxPlayers = 4;                  // 4 by default can vary 
    public float gameStartsAt = 0;

    public int PlayersRequired => minPlayers - currentGamePlayers.Count;

    // In Game info
    public bool gameActive = false;
    public int currentPlayerID = 0;         // it would be better if the currentPlayers what a dict with playerId as the key and name as the value. \n
    public string CurrentPlayerName{
        get{
            if ( currentGamePlayers.ContainsKey( currentPlayerID ) )
                return currentGamePlayers[ currentPlayerID ].nickname;
            else
                return "Error";
        }
    }

    // Connection and game status
    private ConnectionStatus connStatus = ConnectionStatus.None;
    private GameStatus gameStatus = GameStatus.None;


    public bool Connecting => connStatus == ConnectionStatus.Connecting;
    public bool Connected => connStatus == ConnectionStatus.Connected;
    public bool ConnectionError => connStatus == ConnectionStatus.Error;

    public bool JoingGame => gameStatus == GameStatus.Joining;
    public bool GameActive => gameStatus == GameStatus.Active;

    //private void OnEnable ()
    public void Init ()
    {

        if ( inited ) return;   // make sure that this doent happen twice

        connStatus = ConnectionStatus.None; // this is what i hate about scriptables ffs!
        gameStatus = GameStatus.None;

        Protocol.HandleProtocol.Inst.Bind( 'i', ReciveClientIdentityRequest );
        Protocol.HandleProtocol.Inst.Bind( 'r', ReciveClientRegistered );
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

        clientIdentity.nickname = localClient.nickname;

        SocketClient.ActiveSocket.QueueMessage( clientIdentity );

    }

    private void ReciveClientRegistered( Protocol.BaseProtocol protocol )
    {
        Protocol.ClientRegistered registered = protocol as Protocol.ClientRegistered;

        if (registered.ok)
            localClient.reg_key = registered.reg_key;


        Debug.Log( "REG KEY: " + localClient.reg_key );
    }

    private void ReceiveOtherClientStatus ( Protocol.BaseProtocol protocol )
    {

        Protocol.StatusProtocol status = protocol as Protocol.StatusProtocol;

        if ( status.IsType( Protocol.StatusProtocol.Type.Client ) )
        {
            if ( status.ok )
            {
                currentGamePlayers.Add( nextLobbyId, new Client(status.from_client) );
                ++nextLobbyId;
            }
            else
            {
                foreach ( KeyValuePair<int, Client> kp in currentGamePlayers )
                    if ( kp.Value.nickname == status.from_client )
                    {
                        currentGamePlayers.Remove( kp.Key );
                        PlayerDisconnected?.Invoke( kp.Key );
                        break;
                    }
                
            }
        }

        GameInfoUpdated?.Invoke();

    }

    private void ReceiveGameInfo ( Protocol.BaseProtocol protocol )
    {
        Protocol.GameInfoProtocol gameInfo = protocol as Protocol.GameInfoProtocol;

        gameName = gameInfo.game_name;
        minPlayers = gameInfo.min_players;
        maxPlayers = gameInfo.max_players;
        gameStartsAt = Time.time + gameInfo.starts_in;

        currentGamePlayers.Clear();
        nextLobbyId = 0;

        foreach ( string player in gameInfo.players )
        {
            currentGamePlayers.Add( nextLobbyId, new Client(player) );
            ++nextLobbyId;
        }

        GameInfoUpdated?.Invoke();

    }

    private void LaunchGame ( Protocol.BaseProtocol protocol )
    {
        Protocol.LaunchGameProtocol lGame = protocol as Protocol.LaunchGameProtocol;

        currentGamePlayers.Clear(); // starting a new game

        localClient.player_id = lGame.player_id;

        currentGamePlayers.Add( localClient.player_id, localClient );

        SceneManager.LoadScene( "SampleScene", LoadSceneMode.Single );

    }

    private void PreStartGame ( Protocol.BaseProtocol protocol )
    {

        Protocol.PreStartGameProtocol preStartGame = protocol as Protocol.PreStartGameProtocol;
        currentGamePlayers.Clear();

        for ( int i = 0; i < preStartGame.player_ids.Length; i++ )
        {
            int pid = preStartGame.player_ids[ i ];
            Client client = new Client( preStartGame.player_names[ i ] );

            currentGamePlayers.Add( pid, client);
        }

        currentPlayerID = 0;
        PlayersJoined?.Invoke( currentGamePlayers );

    }

    private void StartGame ( Protocol.BaseProtocol protocol )
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

    private void ChangePlayer ( Protocol.BaseProtocol protocol )
    {

        Protocol.ChangePlayerProtocol changePlayer = protocol as Protocol.ChangePlayerProtocol;

        currentPlayerID = changePlayer.player_id;

        GameInfoUpdated?.Invoke();

    }

    /// <summary>
    /// are we the current player
    /// </summary>
    /// <param name="pId">id of the player</param>
    /// <returns></returns>
    public bool PlayerIsActive ()
    {
        return gameActive && currentPlayerID == localClient.player_id;
    }

    /// <summary>
    /// are we pid
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public bool IsPlayer ( int pid )
    {
        return localClient.player_id == pid;
    }

    /// <summary>
    /// are we pid and the current player
    /// </summary>
    /// <param name="pid"></param>
    /// <returns></returns>
    public bool IsPlayerAndActive ( int pid )
    {
        return IsPlayer( pid ) && PlayerIsActive();
    }

    private void ReceiveServerStatus ( Protocol.BaseProtocol protocol )
    {

        Protocol.StatusProtocol serverStatus = protocol as Protocol.StatusProtocol;

        if ( serverStatus.IsType( Protocol.StatusProtocol.Type.Server ) && !serverStatus.ok )
            SetConnectionStatus( ConnectionStatus.Error, serverStatus.message );

    }

    public void SetConnectionStatus ( ConnectionStatus status, string message = "" )
    {
        if ( status != connStatus )
        {
            Debug.Log( "Set connection status " + status );

            ConnectionStatusChanged?.Invoke( status, message );
            connStatus = status;

        }

        // TODO: reset game status??

    }

    public void SetGameStatus ( GameStatus status )
    {

        if ( status != gameStatus )
        {
            Debug.Log( "Set game status " + status );
            gameStatus = status;
            GameStatusChanged?.Invoke( status );
        }

    }

    public void ResetConnectionStatus ()
    {
        connStatus = ConnectionStatus.None;
        ConnectionStatusChanged?.Invoke( ConnectionStatus.None, "" );

        // TODO: reset game status with connection??

    }

    private void OnDestroy ()
    {
        Protocol.HandleProtocol.Inst.Unbind( 'i', ReciveClientIdentityRequest );
        Protocol.HandleProtocol.Inst.Unbind( 'r', ReciveClientRegistered );
        Protocol.HandleProtocol.Inst.Unbind( 's', ReceiveServerStatus );
        Protocol.HandleProtocol.Inst.Unbind( 'd', ReceiveGameInfo );
        Protocol.HandleProtocol.Inst.Unbind( 's', ReceiveOtherClientStatus );
        Protocol.HandleProtocol.Inst.Unbind( 'P', PreStartGame );
        Protocol.HandleProtocol.Inst.Unbind( 'S', StartGame );
        Protocol.HandleProtocol.Inst.Unbind( 'C', ChangePlayer );


    }


    public class Client
    {
        public string client_id;
        public string nickname;

        public Client( string _nickname )
        {
            nickname = _nickname;
        }
    }

    public class LocalClient : Client
    {
        public string reg_key;
        public int player_id;   // the Id of the play when in game. this is assigned when the game is launched
        public LocalClient ( string _nickname ) :
            base(_nickname)
        { }

    }

}
