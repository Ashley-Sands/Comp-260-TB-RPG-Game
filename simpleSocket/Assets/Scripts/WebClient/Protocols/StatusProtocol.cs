using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class StatusProtocol : BaseProtocol
    {
        
        public enum Type { None = -1, Server = 0, Client = 1, Game = 2 }

        public override char Identity => 's';
        public override string Name => "Status Message";

        public int status_type = (int)Type.None;
        public bool ok;
        public string message;

        public Type GetMessageType()
        {
            return (Type)status_type;
        }

        public void SetMessageType( Type statusType )
        {
            status_type = (int)statusType;
        }

        public bool IsType( Type statusType )
        {
            return (int)statusType == status_type;
        }

    }

}