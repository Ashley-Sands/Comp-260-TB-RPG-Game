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

    public class JoinedGameProtocol : BaseProtocol
    {
        public override char Identity => 'J';
        public override string Name => "JoinedGame";

        public string player_name;
        public int player_id;

    }

}