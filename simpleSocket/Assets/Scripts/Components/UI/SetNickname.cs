using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNickname : MonoBehaviour
{
    [SerializeField] private GameData gameData;
    [SerializeField] private TMPro.TMP_InputField nicknameInput;

    public void SetPlayerNickname ()
    {
        gameData.nickname = nicknameInput.text;
    }
}
