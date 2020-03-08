using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RequestAvailableGames : MonoBehaviour
{
    void GetAvailableGames()
    {
        Protocol.GameRequestProtocol gameRequest = new Protocol.GameRequestProtocol();

        SocketClient.ActiveSocket.QueueMessage( gameRequest );

    }
}
