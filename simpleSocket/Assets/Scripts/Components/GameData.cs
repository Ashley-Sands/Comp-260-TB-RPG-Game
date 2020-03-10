using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Player Info
    public string nickname = "player";

    // Game Info
    public string gameName = "";
    public string currentGamePlayers = "";
    public int maxPlayers = 4;                  // 4 by default can vary 

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

        inited = true;

    }

    private void ReciveClientIdentityRequest ( Protocol.BaseProtocol protocol )
    {

        // fill in the info and send it back to the sever
        Protocol.ClientIdentity clientIdentity = protocol as Protocol.ClientIdentity;

        clientIdentity.nickname = nickname;

        SocketClient.ActiveSocket.QueueMessage( clientIdentity as object );

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

    }


}