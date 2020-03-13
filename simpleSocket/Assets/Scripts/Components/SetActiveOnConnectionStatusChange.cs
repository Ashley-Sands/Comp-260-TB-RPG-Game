using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnConnectionStatusChange : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private ConnectionStatus activeStatus = ConnectionStatus.Connected;

    private void Awake ()
    {

        SocketClient.ActiveGameData.ConnectionStatusChanged += ConnectionStatusChanged;

    }

    private void ConnectionStatusChanged ( ConnectionStatus status, string message )
    {

        if ( status == activeStatus )
            obj.SetActive( true );
        else
            obj.SetActive( false );

    }

    private void OnDestroy ()
    {
        SocketClient.ActiveGameData.ConnectionStatusChanged -= ConnectionStatusChanged;
    }
}
