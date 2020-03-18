using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class RequestAvailableGames : MonoBehaviour
{

    private void Awake ()
    {
        HandleProtocol.Inst.Bind( 's', GetAvailableGames );
    }

    void GetAvailableGames( BaseProtocol protocol )
    {

        StatusProtocol serverStatus = protocol as StatusProtocol;

        if ( !serverStatus.IsType( StatusProtocol.Type.Server ) || !serverStatus.ok ) return; // oppsie we've been bad.... :(

        print( "Status Ok: " + serverStatus.ok );
        CurrentLobbyProtocol gameRequest = new CurrentLobbyProtocol();
        SocketClient.ActiveSocket.QueueMessage( gameRequest );

    }

    private void OnDestroy ()
    {
        HandleProtocol.Inst.Unbind( 's', GetAvailableGames );
    }
}
