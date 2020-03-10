using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class LaunchGameProtocol : BaseProtocol
    {
        public override char Identity => 'b';
        public override string Name => "Start Match";

        public int player_id;

    }

}