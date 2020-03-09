using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "GameData", menuName = "GameData" )]
public class GameData : ScriptableObject
{

    public delegate void connectionStatusChanged ( ConnectionStatus status );
    public event connectionStatusChanged ConnectionStatusChanged;           

    public string nickname = "player";

    private ConnectionStatus connStatus = ConnectionStatus.None;

    public bool Connecting => connStatus == ConnectionStatus.Connecting;
    public bool Connected => connStatus == ConnectionStatus.Connected;
    public bool ConnectionError => connStatus == ConnectionStatus.Error;

    
    private void OnEnable ()
    {
        connStatus = ConnectionStatus.None; // this is what i hate about scriptables ffs!

        Protocol.HandleProtocol.Inst.Bind( 'i', ReciveClientIdentityRequest );
        Protocol.HandleProtocol.Inst.Bind( 'S', ReceiveServerStatus );

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

        Protocol.ServerStatusProtocol serverStatus = protocol as Protocol.ServerStatusProtocol;

        if ( !serverStatus.ok )
            SetStatus( ConnectionStatus.Error );

    }

    public void SetStatus( ConnectionStatus status )
    {
        if (status != connStatus)
        {
            Debug.Log( "Set status " + status );

            ConnectionStatusChanged?.Invoke( status );
            connStatus = status;

        }
    }

    private void OnDisable ()
    {
        Protocol.HandleProtocol.Inst.Unbind( 'i', ReciveClientIdentityRequest );
        Protocol.HandleProtocol.Inst.Unbind( 'S', ReceiveServerStatus );

    }


}