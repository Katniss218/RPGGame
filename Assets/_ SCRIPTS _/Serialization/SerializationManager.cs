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
        // The registry can hold uniquely identified objects of any type for referencing during the serialization/deserialization.

        static Dictionary<object, Guid> identGuid = new Dictionary<object, Guid>();
        static Dictionary<Guid, object> identObject = new Dictionary<Guid, object>();

        /// <summary>
        /// Registers the object-identifier pair with the reference registry.
        /// </summary>
        public static void RegisterObject<T>( Guid guid, T obj ) where T : class
        {
            identGuid.Add( obj, guid );
            identObject.Add( guid, obj );
        }

        /// <summary>
        /// Clears the reference registry.
        /// </summary>
        public static void ClearRegistry()
        {
            if( identObject == null )
            {
                identObject = new Dictionary<Guid, object>();
            }
            if( identGuid == null )
            {
                identGuid = new Dictionary<object, Guid>();
            }

            identObject.Clear();
            identGuid.Clear();
        }

        /// <summary>
        /// Gets the identifier for a given object from the reference registry.
        /// </summary>
        public static Guid? GetGuid( object obj )
        {
            if( identGuid.TryGetValue( obj, out Guid guid ) )
            {
                return guid;
            }
            return null;
        }

        /// <summary>
        /// Gets the object for a given identifier from the reference registry.
        /// </summary>
        public static T GetObject<T>( Guid guid ) where T : class
        {
            if( identObject.TryGetValue( guid, out object obj ) )
            {
                return obj as T;
            }
            return null;
        }

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
            ClearRegistry();
            try
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
            finally
            {
                ClearRegistry();
            }
        }

        /// <summary>
        /// Deserializes and loads the scene from a JSON JObject.
        /// </summary>
        public static void LoadScene( JObject json )
        {
            ClearRegistry();
            try
            {
                foreach( var objData in json["Objects"] )
                {
                    SerializationHelper.SpawnAndRegisterGameObject( (JObject)objData );
                }

                foreach( var objData in json["Objects"] )
                {
                    SerializationHelper.SetDataGameObject( GetObject<GameObject>( (Guid)objData["$id"] ), (JObject)objData );
                }
            }
            finally
            {
                ClearRegistry();
            }
        }
    }
}