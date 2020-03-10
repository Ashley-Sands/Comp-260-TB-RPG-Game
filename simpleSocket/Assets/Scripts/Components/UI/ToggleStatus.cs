using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleStatus : MonoBehaviour
{

    [SerializeField] private GameObject connectingUI;
    [SerializeField] private GameObject errorUI;
    [SerializeField] private GameObject holdUI;

    [SerializeField] private GameData gameData;

    private void Awake()
    {

        gameData.ConnectionStatusChanged += ConnectionStatusChanged;

    }

    private void ConnectionStatusChanged( ConnectionStatus status )
    {

        if ( status == ConnectionStatus.Connecting )
            ToggleUiComps( true, false );
        else if ( status == ConnectionStatus.Error )
            ToggleUiComps( false, true );
        else
            ToggleUiComps( false, false );

    }

    private void ToggleUiComps( bool showConnUI, bool showErrorUI )
    {

        holdUI.SetActive( showConnUI || showErrorUI );

        connectingUI.SetActive( showConnUI );
        errorUI.SetActive( showErrorUI );

    }

    private void OnDestroy()
    {
        gameData.ConnectionStatusChanged -= ConnectionStatusChanged;
    }

}
