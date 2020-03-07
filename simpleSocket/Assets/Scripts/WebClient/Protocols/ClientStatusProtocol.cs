using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class ClientStatusProtocol : BaseProtocol
    {

        public override char Idenity => 's';
        public override string Name => "clientStatus";

        public bool connected = false;

    }
}