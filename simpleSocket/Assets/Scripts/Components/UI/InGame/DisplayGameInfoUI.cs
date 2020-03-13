using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayGameInfoUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI nicknameUI;
    [SerializeField] private TextMeshProUGUI currentPlayerUI;
    [SerializeField] private TextMeshProUGUI turnCountdownUI;

    private GameData gameData;

    Coroutine countdownTimer;


    private void Start ()
    {

        gameData = SocketClient.ActiveGameData;

        gameData.GameInfoUpdated += UpdateGameInfo;
        Protocol.HandleProtocol.Inst.Bind( 'C', ChangePlayer );

        nicknameUI.SetText( string.Format( "{0}({1})", gameData.nickname, gameData.playerID ) );

    }

    private void UpdateGameInfo()
    {
         currentPlayerUI.SetText( string.Format( "{0}({1})", gameData.currentPlayerName, gameData.currentPlayerID ) );
    }

    private void ChangePlayer( Protocol.BaseProtocol protocol )
    {
        Protocol.ChangePlayerProtocol changePlayer = protocol as Protocol.ChangePlayerProtocol;

        if ( countdownTimer != null )
            StopCoroutine( countdownTimer );

        countdownTimer = StartCoroutine( Countdown( Time.time + changePlayer.turn_len ) );

    }

    private IEnumerator Countdown ( float endTime )
    {

        float remainTime = endTime - Time.time;

        while ( remainTime > 0f )
        {
            yield return new WaitForSeconds( 1f );
            remainTime = Mathf.Max( 0, endTime - Time.time );

            turnCountdownUI.SetText( string.Format( "{0} secs", Helpers.GetTime( Mathf.FloorToInt( remainTime ) ) ) );

        }

    }

    private void OnDestroy ()
    {
        gameData.GameInfoUpdated -= UpdateGameInfo;
        Protocol.HandleProtocol.Inst.Unbind( 'C', ChangePlayer );

    }

}
