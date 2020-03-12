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

    /// <summary>
    /// sent once the client has loaded into the game
    /// </summary>
    public class JoinedGameProtocol : BaseProtocol
    {
        public override char Identity => 'J';
        public override string Name => "JoinedGame";

        public int player_id;

    }

    /// <summary>
    /// sent from the server once all the clients have joined
    /// </summary>
    public class PreStartGameProtocol : BaseProtocol
    {
        public override char Identity => 'P';
        public override string Name => "PreStartGame";

        public int[] player_ids;
        public string[] player_names;

    }

    /// <summary>
    /// Sent from client once all players (clients) have been loaded into the game
    /// Sent from sever once it has received a copy from each client.
    /// then the game can start :)
    /// </summary>
    public class StartGameProtocol : BaseProtocol
    {

        public override char Identity => 'S';
        public override string Name => "StartGame";

        public bool ok;

    }

}