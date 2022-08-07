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


        /* ### ###  FUTURE OPEN WORLD LOADING SYSTEM  ### ###
        
        Saving in the editor saves the ENTIRE WORLD.

        Saving in game saves the part that is loaded.


        Loading in the editor ---

        Loading in game loads the part around the player (dynamically) as well as any parts that are referenced.
        - Potentially allow loading parts that are nearby to far away quest targets, etc.

        */


        /// <summary>
        /// Serializes and saves the scene to a JSON JObject.
        /// </summary>
        public static JObject SaveScene()
        {
            RPGObject[] allObjects = Object.FindObjectsOfType<RPGObject>(); //find all the persistent objects in the level

            JArray dataJson = new JArray();
#warning TODO - How do we deal with circular dependencies in serialized objects?
            // - Assign guids to everything that is referenced first.
            // - Serialize everything later.

            foreach( var obj in allObjects )
            {
#warning TODO - move the player to be a normal object in the scene, since there will only be one scene.
                if( SerializationHelper.ShouldSerialize( obj.gameObject ) )
                {
                    try
                    {
                        JObject data = SerializationHelper.GetDataRpgObject( obj );
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
                (RPGObject go, Guid guid) = SerializationHelper.SpawnRpgObject( (JObject)objData );
            }

            foreach( var objData in json["Objects"] )
            {
                SerializationHelper.SetDataGameObject( RPGObject.Get( (Guid)objData["$id"] ), (JObject)objData );
            }
        }
    }
}