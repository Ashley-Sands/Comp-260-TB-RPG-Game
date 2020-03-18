using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNickname : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_InputField nicknameInput;

    public void SetPlayerNickname ()
    {
        SocketClient.ActiveGameData.localClient.nickname = nicknameInput.text;
    }
}
