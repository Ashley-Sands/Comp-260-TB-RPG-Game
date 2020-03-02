using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Protocol
{

    public abstract class BaseProtocol
    {
        public abstract char Idenity { get; }
        public abstract string Name { get; }

        private int chached_jsonLength = 0;

        /// <summary>
        /// Gets this class in json format :)
        /// </summary>
        /// <param name="jsonLength">OUT the length of the message</param>
        /// <returns>this class as json</returns>
        public string GetJson (out int jsonLength)
        {
            string jsonStr = JsonUtility.ToJson( this );
            chached_jsonLength = jsonLength = jsonStr.Length;

            return jsonStr;
        }

        /// <summary>
        /// Gets the chached message length from the last time GetMessage was called
        /// </summary>
        /// <returns>chached message length</returns>
        public int GetJsonLength ()
        {
            return chached_jsonLength;
        }

    }


}