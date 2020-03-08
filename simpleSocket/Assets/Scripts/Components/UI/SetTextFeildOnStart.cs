using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTextFeildOnStart : MonoBehaviour
{

    [SerializeField] private TMP_InputField textInput;
    [SerializeField] private RandomNames randomNames;

    void Start ()
    {
        GetRandomName();
    }

    public void GetRandomName()
    { 
        if ( textInput == null || randomNames == null)
        {
            Debug.LogError("Unable to set random name, either textinput or randomnames is missing");
            return;
        }

        textInput.text = randomNames.GetRandomName() ;

    }

}
