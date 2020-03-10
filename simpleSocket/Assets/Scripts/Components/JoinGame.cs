using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

public class JoinServer : MonoBehaviour
{

    private void Awake ()
    {
        HandleProtocol.Inst.Bind( 's', JoinGame );
    }

    void JoinGame( BaseProtocol protocol )
    {

        StatusProtocol status = protocol as StatusProtocol;

        if (status.IsType(StatusProtocol.Type.Game) && status.ok)
        {
            // 
        }

    }

}
