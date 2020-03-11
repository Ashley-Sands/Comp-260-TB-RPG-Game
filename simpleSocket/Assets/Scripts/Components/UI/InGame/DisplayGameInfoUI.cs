using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayGameInfoUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI nicknameUI;
    [SerializeField] private TextMeshProUGUI currentPlayerUI;

    private GameData gameData;

    private void Start ()
    {
        gameData = SocketClient.ActiveGameData;

        gameData.GameInfoUpdated += UpdateGameInfo;

        nicknameUI.SetText( string.Format( "{0}({1})", gameData.nickname, gameData.playerID ) );

    }

    private void UpdateGameInfo()
    {
         currentPlayerUI.SetText( string.Format( "{0}({1})", gameData.currentPlayerName, gameData.currentPlayerID ) );
    }

    private void OnDestroy ()
    {
        gameData.GameInfoUpdated -= UpdateGameInfo;
    }

}
