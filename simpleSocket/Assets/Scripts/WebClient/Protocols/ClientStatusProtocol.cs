using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class ClientIdentity : BaseProtocol
    {
        public override char Identity => 'i';
        public override string Name => "Client Identity";

        public string nickname = "";
        public string reg_key = "";

    }

    public class ClientRegistered : BaseProtocol
    {

        public override char Identity => 'r';
        public override string Name => "Client Registered";

        public bool ok;
        public string client_id = "";
        public string reg_key = "";

    }

}