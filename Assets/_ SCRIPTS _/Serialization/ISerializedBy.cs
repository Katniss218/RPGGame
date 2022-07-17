using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Serialization
{
    public interface ISerializedBy
    {
        /// <summary>
        /// Returns the serialized data for this object.
        /// </summary>
        JObject GetData();

        /// <summary>
        /// Applies the serialized data to this object.
        /// </summary>
        void SetData( JObject data );
    }
}