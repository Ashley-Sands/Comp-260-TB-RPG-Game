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

}