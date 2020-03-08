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

    }

    public class ClientStatusProtocol : BaseProtocol
    {

        public override char Identity => 's';
        public override string Name => "clientStatus";

        public bool connected = false;

    }

    public class ServerStatusProtocol : BaseProtocol
    {
        public override char Identity => 'S';
        public override string Name => "serverStatus";

        public bool ok = false;

    }
}