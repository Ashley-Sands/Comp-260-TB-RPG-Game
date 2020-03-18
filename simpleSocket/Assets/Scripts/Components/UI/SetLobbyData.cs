using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetLobbyData : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI gameName; 
    [SerializeField] private TextMeshProUGUI clients;
    [SerializeField] private TextMeshProUGUI details;
    [SerializeField] private float startsAt = 0;

    bool countdownIsRunning;

    private void Awake ()
    {
        SocketClient.ActiveGameData.GameInfoUpdated += UpdateGameInfo;

        UpdateGameInfo( );

    }

    private void UpdateGameInfo ( )
    {
        GameData gameData = SocketClient.ActiveGameData;
        startsAt = gameData.gameStartsAt;

        if ( gameData.PlayersRequired > 0 )
        {
            DetailsMorePlayersUi();
        }
        else if ( !countdownIsRunning )
        {
            StartCoroutine( Countdown() );
        }

        UpdateUI();

    }

    private IEnumerator Countdown()
    {

        countdownIsRunning = true;

        GameData gameData = SocketClient.ActiveGameData;
        float remainTime = startsAt - Time.time;

        while ( remainTime > 0f )
        {
            yield return new WaitForSeconds( 1f );
            remainTime = Mathf.Max(0, startsAt - Time.time);

            details.SetText( string.Format( "Match Starts In {0} secs", Helpers.GetTime( Mathf.FloorToInt(remainTime) ) ) );

            if ( gameData.PlayersRequired > 0 )
            {
                DetailsMorePlayersUi(); 
                break;
            }


        }

        countdownIsRunning = false;

    }

    private void UpdateUI()
    {
        GameData gameData = SocketClient.ActiveGameData;

        gameName.SetText( gameData.gameName );

        string clientList = "";

        foreach ( KeyValuePair<int, GameData.Client> c in gameData.currentGamePlayers )
            clientList += c.Value.nickname + "\n";

        clients.SetText( clientList );

    }

    private void DetailsMorePlayersUi( )
    {
        GameData gameData = SocketClient.ActiveGameData;

        details.SetText( string.Format( " Require {0} more players", gameData.PlayersRequired ) );
    }

    private void OnDestroy ()
    {
        SocketClient.ActiveGameData.GameInfoUpdated -= UpdateGameInfo;

    }


}
