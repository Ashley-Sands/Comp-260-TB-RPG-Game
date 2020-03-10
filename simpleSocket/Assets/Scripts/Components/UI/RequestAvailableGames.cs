using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class RequestAvailableGames : MonoBehaviour
{

    private void Awake ()
    {
        HandleProtocol.Inst.Bind( 'S', GetAvailableGames );
    }

    void GetAvailableGames( BaseProtocol protocol )
    {

        StatusProtocol serverStatus = protocol as StatusProtocol;

        if ( serverStatus.IsType( StatusProtocol.Type.Server ) && !serverStatus.ok ) return; // oppsie we've been bad.... :(

        GameRequestProtocol gameRequest = new GameRequestProtocol();
        SocketClient.ActiveSocket.QueueMessage( gameRequest );

    }

    private void OnDestroy ()
    {
        HandleProtocol.Inst.Unbind( 'S', GetAvailableGames );
    }
}
