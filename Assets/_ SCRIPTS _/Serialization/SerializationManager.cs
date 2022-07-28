using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace RPGGame.Serialization
{
    /// <summary>
    /// Manages the scene saving and loading (JSON).
    /// </summary>
    public static class SerializationManager
    {
        // Serialization methods:
        // - implicit/explicit pair Operator Overloading:
        //   - assets
        //   - standalone classes
        // - ISerializedComponent interface (Get/Set Data methods)
        //   - monobehaviours / components

        /// <summary>
        /// Serializes and saves the scene to a JSON JObject.
        /// </summary>
        public static JObject SaveScene()
        {
            GameObject[] allObjects = Object.FindObjectsOfType<GameObject>(); //find all the persistent objects in the level

            JArray dataJson = new JArray();
            foreach( var obj in allObjects )
            {
                if( SerializationHelper.ShouldSerialize( obj ) )
                {
                    try
                    {
                        JObject data = SerializationHelper.GetDataGameObject( obj.gameObject );
                        dataJson.Add( data );
                    }
                    catch( Exception ex )
                    {
                        Debug.LogException( ex );
                    }
                }
            }

            JObject json = new JObject()
            {
                { "Objects", dataJson }
            };

            return json;
        }

        /// <summary>
        /// Deserializes and loads the scene from a JSON JObject.
        /// </summary>
        public static void LoadScene( JObject json )
        {
            foreach( var objData in json["Objects"] )
            {
                SerializationHelper.SpawnAndRegisterGameObject( (JObject)objData );
            }

            foreach( var objData in json["Objects"] )
            {
                SerializationHelper.SetDataGameObject( RPGObject.Get( (Guid)objData["$id"] ).gameObject, (JObject)objData );
            }
        }
    }
}