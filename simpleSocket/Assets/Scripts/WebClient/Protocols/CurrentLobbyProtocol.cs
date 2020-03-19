using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class CurrentLobbyProtocol : BaseProtocol
    {
        public override char Identity => 'g';
        public override string Name => "Current";

        public int LobbyCount => lobby_ids != null ? lobby_ids.Length : 0;

        public int[] lobby_ids;
        public string[] level_names;
        public int[] min_players;
        public int[] max_players;
        public int[] current_players;

    }

    /// <summary>
    /// Sent from the client to the server requesting to join lobby_id
    /// Then returned by the server containing the host and port to connect to
    /// </summary>
    public class JoinLobbyProtocol : BaseProtocol   // See start game for joined game
    {
        
        public override char Identity => 'j';
        public override string Name => "Join Match";

        public int lobby_id = -1;

        public string host;
        public int port;

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