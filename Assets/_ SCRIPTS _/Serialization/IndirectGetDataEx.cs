using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    /// <summary>
    /// A helper class with Get/Set Data extension methods, for the classes that can't have the Get/Set Data.
    /// </summary>
    public static class IndirectGetDataEx
    {
        // Transform

        public static JObject GetData( this Transform transform )
        {
            return new JObject()
            {
                { "Position", transform.position.ToJson() },
                { "Rotation", transform.rotation.ToJson() },
                { "LocalScale", transform.localScale.ToJson() }
            };
        }

        public static void SetData( this Transform transform, JObject data )
        {
            transform.position = data["Position"].ToVector3();
            transform.rotation = data["Rotation"].ToQuaternion();
            transform.localScale = data["LocalScale"].ToVector3();
        }

        // 
    }
}
