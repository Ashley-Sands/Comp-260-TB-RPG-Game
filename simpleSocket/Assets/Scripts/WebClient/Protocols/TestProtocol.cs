using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{

	class PingProtocol : BaseProtocol
	{
		public override char Identity => '&';
		public override string Name => "PING";

		// i cant grantee that local and server times are the same.
		public double client_send_time = -1;
		public double server_receive_time = -1;

		public PingProtocol (double millisSinceEpoch )
		{

			client_send_time = millisSinceEpoch;

		}
	}

}  