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
		public int client_send_time = -1;
		public int server_receive_time = -1;

		public PingProtocol ()
		{
			TimeSpan t = DateTime.UtcNow - new DateTime( 1970, 1, 1 );
			int millisSinceEpoch = (int)t.TotalMilliseconds;

			client_send_time = millisSinceEpoch;

		}
	}

}