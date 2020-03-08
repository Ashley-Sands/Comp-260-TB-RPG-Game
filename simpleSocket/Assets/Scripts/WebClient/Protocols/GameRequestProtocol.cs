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
}