using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameObjectActive : MonoBehaviour
{
    [SerializeField] private GameObject[] gameObj;

    public void DisableUI ()
    {
        for ( int i = 0; i < gameObj.Length; i++ )
            gameObj[ i ].SetActive( false );
    }

    public void EnableUI ()
    {
        for ( int i = 0; i < gameObj.Length; i++ )
            gameObj[ i ].SetActive( true );
    }
}
