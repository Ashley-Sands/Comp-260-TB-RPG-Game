using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNickname_fromGameData : MonoBehaviour
{

    [SerializeField] private TMPro.TextMeshProUGUI text;

    private void Start ()
    {
        if ( text == null )
        {
            Debug.LogError( "Unable to set random name, text is missing" );
            return;
        }

        text.SetText( SocketClient.ActiveGameData.CurrentPlayerName );
    }
}
