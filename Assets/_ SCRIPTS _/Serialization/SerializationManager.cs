using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using RPGGame.ObjectCreation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

namespace RPGGame.Serialization
{
    public static class SerializationManager
    {
        static Dictionary<object, Guid> identGuid = new Dictionary<object, Guid>();
        static Dictionary<Guid, object> identObject = new Dictionary<Guid, object>();

        public static void RegisterObject<T>( Guid guid, T obj ) where T : class
        {
            identGuid.Add( obj, guid );
            identObject.Add( guid, obj );
        }

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

        public static Guid? GetGuid( object obj )
        {
            if( identGuid.TryGetValue( obj, out Guid guid ) )
            {
                return guid;
            }
            return null;
        }

        public static T GetObject<T>( Guid guid ) where T : class
        {
            if( identObject.TryGetValue( guid, out object obj ) )
            {
                return obj as T;
            }
            return null;
        }

        // objects that are not components (basically standalone classes/structs) are serialized via operator overloading.
        // components are serialized via the interface.
        // Everything that goes onto a gameobject is serialized by the Get/Set Data methods.

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