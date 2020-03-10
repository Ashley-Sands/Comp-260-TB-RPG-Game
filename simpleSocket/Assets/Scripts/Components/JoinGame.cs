using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Protocol;

public class JoinGame : MonoBehaviour
{

    private void Awake ()
    {
        HandleProtocol.Inst.Bind( 's', Join_Game );
    }

    void Join_Game( BaseProtocol protocol )
    {

        StatusProtocol status = protocol as StatusProtocol;

        if ( status.IsType(StatusProtocol.Type.Game) && status.ok)
        {
            SceneManager.LoadScene( "GameLobby", LoadSceneMode.Single );
            SocketClient.ActiveGameData.SetGameStatus( GameData.GameStatus.Active ); 
        }

    }

    private void OnDestroy ()
    {
        HandleProtocol.Inst.Unbind( 's', Join_Game );

    }

}
