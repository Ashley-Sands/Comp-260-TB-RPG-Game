using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleGameObjectActive_OnBoolEvent : MonoBehaviour
{

    [SerializeField] private DelegateEvent_Bool displayObjectEvent;
    [SerializeField] private GameObject[] objs;


    void Start()
    {
        displayObjectEvent.scriptableEvent += DisplayObjects;
    }

    private void DisplayObjects( bool status )
    {

        foreach ( GameObject go in objs )
            go.SetActive( !status );

    }

    private void OnDestroy ()
    {
        displayObjectEvent.scriptableEvent -= DisplayObjects;
    }


}
