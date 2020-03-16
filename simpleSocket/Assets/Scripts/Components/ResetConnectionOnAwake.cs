using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetConnectionOnAwake : MonoBehaviour
{
    private void Awake ()
    {

        SocketClient.ActiveSocket.Running = false;
        SocketClient.ActiveGameData.ResetConnectionStatus();

    }
}
