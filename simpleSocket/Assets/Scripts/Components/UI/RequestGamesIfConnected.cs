using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class RequestGamesIfConnected : MonoBehaviour
{
    [SerializeField] bool attamptToClearDocketError = true;
    [SerializeField] bool autoReload = false;
    [SerializeField] bool onStart = false;
    [SerializeField] int reloadIntervals = 30; // seconds

    private bool invoking = false;

    private void Awake ()
    {
        if ( attamptToClearDocketError && SocketClient.ActiveGameData.ConnectionError )
            SocketClient.ActiveGameData.SetConnectionStatus( ConnectionStatus.Connected );
            
        HandleProtocol.Inst.Bind( 's', GetAvailableGames );
    }

    private void Start ()
    {

        if (onStart)
            StartRequesting();

    }

    public void StartRequesting()
    {

        if ( autoReload && !invoking )
        {
            InvokeRepeating( "RequestGamesList", 1, reloadIntervals );
            invoking = true;
        }
        else if ( !invoking )
        {
            RequestGamesList();
        }
    }

    public void RequestGamesList()
    {

        if (SocketClient.ActiveGameData.Connected)
        {
            GameRequestProtocol gameRequest = new GameRequestProtocol();
            SocketClient.ActiveSocket.QueueMessage( gameRequest );
        }

    }

    void GetAvailableGames ( BaseProtocol protocol )
    {

        StatusProtocol serverStatus = protocol as StatusProtocol;

        if ( !serverStatus.IsType( StatusProtocol.Type.Server ) || !serverStatus.ok ) return; // oppsie we've been bad.... :(

        print( "Status Ok: " + serverStatus.ok );
        StartRequesting();

    }

    private void OnDestroy ()
    {
        HandleProtocol.Inst.Unbind( 's', GetAvailableGames );
    }

}
