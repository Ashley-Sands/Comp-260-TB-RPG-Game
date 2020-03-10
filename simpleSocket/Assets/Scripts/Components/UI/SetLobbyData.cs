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

        startsAt = SocketClient.ActiveGameData.gameStartsAt;

        if ( !countdownIsRunning )
            StartCoroutine( Countdown() );

        UpdateUI();

    }

    private IEnumerator Countdown()
    {

        countdownIsRunning = true;

        float remainTime = startsAt - Time.time;

        while ( remainTime > 0f )
        {
            yield return new WaitForSeconds( 1f );
            remainTime = Mathf.Max(0, startsAt - Time.time);

            details.SetText( string.Format( "Match Starts In {0} secs", Helpers.GetTime( Mathf.FloorToInt(remainTime) ) ) );

        }

        countdownIsRunning = false;

    }

    private void UpdateUI()
    {
        GameData gameData = SocketClient.ActiveGameData;

        gameName.SetText( gameData.gameName );
        clients.SetText( string.Join( "\n", gameData.currentGamePlayers ) );

    }

    private void OnDestroy ()
    {
        SocketClient.ActiveGameData.GameInfoUpdated -= UpdateGameInfo;

    }


}
