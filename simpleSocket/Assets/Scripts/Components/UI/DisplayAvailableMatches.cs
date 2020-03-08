using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayAvailableMatches : ScriptableObject
{

    public string nickname = "player";

    private void OnEnable ()
    {
        Protocol.HandleProtocol.Inst.Bind( 'i', ReciveClientIdentityRequest );
    }

    private void ReciveClientIdentityRequest( Protocol.BaseProtocol protocol )
    {

        // fill in the info and send it back to the sever
        Protocol.ClientIdentity clientIdentity = protocol as Protocol.ClientIdentity;

        clientIdentity.nickname = nickname;

        SocketClient.ActiveSocket.QueueMessage( clientIdentity as object );

    }

    private void OnDisable ()
    {
        Protocol.HandleProtocol.Inst.Unbind( 'i', ReciveClientIdentityRequest );

    }


}
