using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendPingInput : MonoBehaviour
{

    public GameObject pinging_HUD;

    void Update()
    {
        
        if ( Input.GetKeyDown( KeyCode.P ) )
        {

            InvokeRepeating( "PingGame", 0, 0.5f );
            pinging_HUD.SetActive( true );
        }

        if ( Input.GetKeyDown( KeyCode.O ) )
        { 
            CancelInvoke( "PingGame" );
            pinging_HUD.SetActive( false );
        }

    }

    void PingGame()
    {
        TimeSpan t = DateTime.UtcNow - new DateTime( 1970, 1, 1 );
        double millisSinceEpoch = t.TotalMilliseconds;

        SocketClient.ActiveSocket.QueueMessage( new Protocol.PingProtocol( millisSinceEpoch ) );
    }

}
