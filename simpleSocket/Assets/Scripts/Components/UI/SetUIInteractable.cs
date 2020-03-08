using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetUIInteractable : MonoBehaviour
{

    [SerializeField] private Selectable[] uiElements;

    public void DisableUI()
    {
        for ( int i = 0; i < uiElements.Length; i++ )
            uiElements[ i ].interactable = false;
    }

    public void EnableUI ()
    {
        for ( int i = 0; i < uiElements.Length; i++ )
            uiElements[ i ].interactable = true;
    }

}
