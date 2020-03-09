using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveOnConnectionStatusChange : MonoBehaviour
{
    [SerializeField] private GameObject obj;
    [SerializeField] private ConnectionStatus activeStatus = ConnectionStatus.Connected;

    [SerializeField] private GameData gameData;

    private void Awake ()
    {

        gameData.ConnectionStatusChanged += ConnectionStatusChanged;

    }

    private void ConnectionStatusChanged ( ConnectionStatus status )
    {

        if ( status == activeStatus )
            obj.SetActive( true );
        else
            obj.SetActive( false );

    }

    private void OnDestroy ()
    {
        gameData.ConnectionStatusChanged -= ConnectionStatusChanged;
    }
}
