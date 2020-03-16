using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleStatus : MonoBehaviour
{
    [SerializeField] private DelegateEvent_Bool statusChangedEvent; // this is invoked when even the message is updated.
    [SerializeField] private GameObject holdUI;
    [SerializeField] private TMPro.TextMeshProUGUI messageUI;

    private Dictionary<string, string> messages;



    private void Start()
    {
        messages = new Dictionary<string, string>();
        SocketClient.ActiveGameData.ConnectionStatusChanged += ConnectionStatusChanged;
        SocketClient.ActiveGameData.GameStatusChanged += GameStatusChanged;

    }

    private void ConnectionStatusChanged( ConnectionStatus status, string message )
    {

        switch( status )
        {
            case ConnectionStatus.Connecting:
                AddMessage( "Connecting", "..." );
                break;
            case ConnectionStatus.Error:
                AddMessage( "Error", message );
                break;
            default:
                RemoveMessage( "Connecting", "Error" );
                break;
        }

    }

    private void GameStatusChanged( GameData.GameStatus status )
    {

        switch ( status )
        {
            case GameData.GameStatus.Joining:
                AddMessage( "Joining", "Please wait..." );
                break;
            default:
                RemoveMessage( "Joining" );
                break;
        }

    }

    private void UpdateMessageUI( )
    {

        bool hasMessageToDisplay = messages.Count > 0;
        // Only show the message window if there is a message to be displayed.
        holdUI.SetActive( hasMessageToDisplay );
        statusChangedEvent.Invoke( hasMessageToDisplay );

        if ( !hasMessageToDisplay ) return;

        string msgToDisplay = "";

        foreach (KeyValuePair<string, string> pair in messages)
            msgToDisplay = string.Concat( msgToDisplay, pair.ToString(), "\n" );

        messageUI.SetText( msgToDisplay );

    }

    public void AddMessage ( string typeName, string message, float displayLength = -1 )
    {
        if ( !messages.ContainsKey( typeName ) )
        {
            messages.Add( typeName, message );
        }
        else
        {
            messages[ typeName ] = message;
        }

        if ( displayLength > 0 )
            StartCoroutine( RemoveMessageIn( typeName, displayLength) );

        UpdateMessageUI();

    }

    IEnumerator RemoveMessageIn(string typeName, float delayLength)
    {

        yield return new WaitForSeconds(delayLength);

        RemoveMessage( typeName );

    }

    public void RemoveMessage( params string[] typeName )
    {
        foreach (string t in typeName)
            if ( messages.ContainsKey( t ) )
                messages.Remove( t );

        UpdateMessageUI();

    }

    public void ClearMessages()
    {
        messages.Clear();
        UpdateMessageUI();

    }

    private void OnDestroy()
    {
        SocketClient.ActiveGameData.ConnectionStatusChanged -= ConnectionStatusChanged;
        SocketClient.ActiveGameData.GameStatusChanged -= GameStatusChanged;

    }

}
