﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class MovePlayerProtocols : BaseProtocol
    {
        public override char Identity => 'M';
        public override string Name => "move player";

        public int player_id;

        [SerializeField] private int x;
        [SerializeField] private int y;
        [SerializeField] private int z;

        public Vector3 position {
            get { return new Vector3( x, y, z ); }
            set {
                x = (int)value.x;
                y = (int)value.y;
                z = (int)value.z;
            }
        }

    }
}