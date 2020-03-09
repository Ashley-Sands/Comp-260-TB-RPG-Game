using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetStartSocket : MonoBehaviour
{
    public void ConnectSocket()
    {
        SocketClient.ActiveSocket.Running = true;
    }
}
