using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ConnectionStatus { None, Connecting, Connected, Error }


[CreateAssetMenu( fileName = "GameData", menuName = "GameData" )]
public class GameData : ScriptableObject
{

    public delegate void connectionStatusChanged ( ConnectionStatus status );
    public event connectionStatusChanged ConnectionStatusChanged;               // Make static ??

    public string nickname = "player";

    private ConnectionStatus connStatus = ConnectionStatus.None;

    public bool Connecting => connStatus == ConnectionStatus.Connecting;
    public bool Connected => connStatus == ConnectionStatus.Connected;
    public bool ConnectionError => connStatus == ConnectionStatus.Error;

    
    private void OnEnable ()
    {
        Protocol.HandleProtocol.Inst.Bind( 'i', ReciveClientIdentityRequest );
    }

    private void ReciveClientIdentityRequest ( Protocol.BaseProtocol protocol )
    {

        // fill in the info and send it back to the sever
        Protocol.ClientIdentity clientIdentity = protocol as Protocol.ClientIdentity;

        clientIdentity.nickname = nickname;

        SocketClient.ActiveSocket.QueueMessage( clientIdentity as object );

    }

    public void SetStatus( ConnectionStatus status )
    {
        if (status != connStatus)
        {

            ConnectionStatusChanged.Invoke( status );
            connStatus = status;

        }
    }

    private void OnDisable ()
    {
        Protocol.HandleProtocol.Inst.Unbind( 'i', ReciveClientIdentityRequest );

    }


}