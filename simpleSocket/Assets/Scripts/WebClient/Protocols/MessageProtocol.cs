﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{
    public class MessageProtocol : BaseProtocol
    {
        public override char Identity => 'm';
        public override string Name => "sendMessage";

        public string message = "";

        public MessageProtocol( string _message )
        {
            message = _message;
        }

    }
}