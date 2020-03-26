using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPingInput : MonoBehaviour
{

    public double ddd;

    void Update()
    {
        
        if ( Input.GetKeyDown( KeyCode.P ) )
        {

            TimeSpan t = DateTime.UtcNow - new DateTime( 1970, 1, 1 );
            double millisSinceEpoch = t.TotalMilliseconds;

            SocketClient.ActiveSocket.QueueMessage( new Protocol.PingProtocol( millisSinceEpoch ) );
            print( "Send" + DateTime.UtcNow.ToShortDateString() + " :: "+ millisSinceEpoch + " :: "+ (uint)millisSinceEpoch + " :: " + new DateTime( 1970, 1, 1 ).ToShortDateString() );
        }

    }
}
