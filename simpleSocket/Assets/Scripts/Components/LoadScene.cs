﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public string sceneToLoad;
    [SerializeField] private bool loadOnStart = false;

    void Start()
    {

        if ( loadOnStart )
            SceneManager.LoadScene( sceneToLoad, LoadSceneMode.Single );

    }

    public void LoadLevel()
    {
        SceneManager.LoadScene( sceneToLoad, LoadSceneMode.Single );
    }
}