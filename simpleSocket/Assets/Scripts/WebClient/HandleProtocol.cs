using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Protocol 
{

    public delegate void protocol_event ( BaseProtocol protocol );

    public class HandleProtocol
    {

        private class ProtocolEvent
        {
            public event protocol_event callback;

            public void Invoke ( BaseProtocol protocol )
            {
                callback?.Invoke( protocol );
            }

        }

        private static HandleProtocol inst;
        public static HandleProtocol Inst {
            get {
                if ( inst == null )
                    inst = new HandleProtocol();

                return inst;
            }

        }

        Dictionary<char, ProtocolEvent> protocolEvents;

        private HandleProtocol ()
        {
            protocolEvents = new Dictionary<char, ProtocolEvent>
            {    
                { 'm', new ProtocolEvent() },   // message
                { 's', new ProtocolEvent() },   // client status
                { 'i', new ProtocolEvent() }    // client identity
                // Dont forget to add it to Convert json as well :)
            };

        }

        /// <summary>
        /// Binds function to protocol callback
        /// </summary>
        /// <param name="idenity">the idenity to bind to</param>
        /// <param name="protocolFunc">function to bind</param>
        public void Bind( char idenity, protocol_event protocolFunc )
        {

            if (!protocolEvents.ContainsKey(idenity))
            {
                Debug.LogErrorFormat( "Unable to bind, Failed to identify protocol {0}", idenity );
                return;
            }

            protocolEvents[ idenity ].callback += protocolFunc;

        }

        /// <summary>
        /// Unbinds function to protocol callback
        /// </summary>
        /// <param name="idenity">the idenity to bind to</param>
        /// <param name="protocolFunc">function to bind</param>
        public void Unbind ( char idenity, protocol_event protocolFunc )
        {

            if ( !protocolEvents.ContainsKey( idenity ) )
            {
                Debug.LogErrorFormat( "Unable to unbind, Failed to identify protocol {0}", idenity );
                return;
            }

            protocolEvents[ idenity ].callback -= protocolFunc;

        }

        public void InvokeProtocol( BaseProtocol proto )
        {
            protocolEvents[ proto.Identity ].Invoke( proto );
        }

        /// <summary>
        /// Handles json string as idenity.
        /// </summary>
        /// <param name="idenity">idenity of the json string</param>
        /// <param name="json">json string of idenity</param>
        /// <returns> protocol. null if protocol does not exist</returns>
        public static BaseProtocol ConvertJson ( char idenity, string json)
        {

            BaseProtocol newProto;

            switch ( idenity )
            {
                case 'm':   // message 
                    newProto = JsonUtility.FromJson<MessageProtocol>( json );
                    break;
                case 's':   // client status
                    newProto = JsonUtility.FromJson<ClientStatusProtocol>( json );
                    break;
                case 'i':   // client status
                    newProto = JsonUtility.FromJson<ClientIdentity>( json );
                    break;
                default:    // Not found
                    Debug.LogErrorFormat( "Unable to handle json, Failed to identify protocol {0}", idenity );
                    return null;
            }

            return newProto;

        }



    }

}