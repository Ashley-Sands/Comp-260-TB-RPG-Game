using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Protocol;

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

        Dictionary<char, ProtocolEvent> protocolEvents;
        
        /// <summary>
        /// Initial setup
        /// </summary>
        void Init()
        {
            protocolEvents = new Dictionary<char, ProtocolEvent>
            {
                { 'm', new ProtocolEvent() }
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

        /// <summary>
        /// Handles json string as idenity
        /// </summary>
        /// <param name="idenity">idenity of the json string</param>
        /// <param name="json">json string of idenity</param>
        public void HandleJson ( char idenity, string json)
        {

            BaseProtocol newProto;

            switch ( idenity )
            {
                case 'm':   // message 
                    newProto = JsonUtility.FromJson<MessageProtocol>( json );
                    break;
                default:    // Not found
                    Debug.LogErrorFormat( "Unable to handle json, Failed to identify protocol {0}", idenity );
                    return;
            }

            if ( newProto != null )
                protocolEvents[idenity].Invoke( newProto );

        }



    }

}