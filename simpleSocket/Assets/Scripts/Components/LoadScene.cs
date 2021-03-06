﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public enum LoadEvent{ Start, ServerError, ClientRegistered }
    [SerializeField] private string sceneToLoad;
    [SerializeField] private LoadEvent loadOn = LoadEvent.Start;

    void Start ()
    {

        switch ( loadOn )
        {

            case LoadEvent.Start:
                SceneManager.LoadScene( sceneToLoad, LoadSceneMode.Single );
                break;
            case LoadEvent.ServerError:
                Protocol.HandleProtocol.Inst.Bind( 's', ServerError );
                break;
            case LoadEvent.ClientRegistered:
                Protocol.HandleProtocol.Inst.Bind( 'r', clientRegistered );
                break;

        }
    }

    public void ServerError( Protocol.BaseProtocol protocol )
    {

        Protocol.StatusProtocol status = protocol as Protocol.StatusProtocol;

        if ( status.IsType( Protocol.StatusProtocol.Type.Server ) && !status.ok )
            LoadLevel();

    }

    public void clientRegistered( Protocol.BaseProtocol protocol )
    {

        Protocol.ClientRegistered registered = protocol as Protocol.ClientRegistered;

        if ( registered.ok )
            LoadLevel();

    }

    public void LoadLevel()
    {
        SceneManager.LoadScene( sceneToLoad, LoadSceneMode.Single );
    }

    private void OnDestroy ()
    {
        switch ( loadOn )
        {

            case LoadEvent.ServerError:
                Protocol.HandleProtocol.Inst.Unbind( 's', ServerError );
                break;
            case LoadEvent.ClientRegistered:
                Protocol.HandleProtocol.Inst.Unbind( 'r', clientRegistered );
                break;

        }
    }

}
