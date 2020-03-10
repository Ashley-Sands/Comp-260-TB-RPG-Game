using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class GameRequestProtocol : BaseProtocol
    {
        public override char Identity => 'g';
        public override string Name => "Game Request";

        public string[] available_games;
        public string[] available_slots;
        
    }

    public class JoinGameProtocol : BaseProtocol
    {

        public override char Identity => 'j';
        public override string Name => "Join Match";

        public string match_name = "";

    }

    public class LeaveGameProtocol : BaseProtocol
    {
        public override char Identity => 'l';
        public override string Name => "Leave Match";

    }

    public class GameDataProtocol : BaseProtocol
    {
        public override char Identity => 'd';
        public override string Name => "Match Data";

        public string[] players;

    }
}