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

    public class JoinLobbyProtocol : BaseProtocol   // See start game for joined game
    {

        public override char Identity => 'j';
        public override string Name => "Join Match";

        public string match_name = "";

    }

    /// <summary>
    /// Leaves both game and lobby?
    /// </summary>
    public class LeaveGameProtocol : BaseProtocol   // TODO: implerment
    {
        public override char Identity => 'l';
        public override string Name => "Leave Match";

    }

    /// <summary>
    /// Sent from the server to the client with the basic game info
    /// </summary>
    public class GameInfoProtocol : BaseProtocol
    {
        public override char Identity => 'd';
        public override string Name => "Match Data";

        public string game_name;
        public string[] players;
        public int min_players;
        public int max_players;
        public float starts_in;

    }
}