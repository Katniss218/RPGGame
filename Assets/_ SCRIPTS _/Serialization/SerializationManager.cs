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

namespace RPGGame.Serialization
{
    public static class SerializationManager
    {
        public static Dictionary<object, Guid> identGuid = new Dictionary<object, Guid>();
        public static Dictionary<Guid, object> identObject = new Dictionary<Guid, object>();

        // objects that are not components (basically standalone classes/structs) are serialized via operator overloading.
        // components are serialized via the interface.
        // Everything that goes onto a gameobject is serialized by the Get/Set Data methods.

        public static JObject SaveScene()
        {
            JObject obj = new JObject()
            {
                { "Objects", SaveSceneObjects() }
            };

            return obj;
        }

        public static void LoadScene( JObject json )
        {
            try
            {
                foreach( var objData in json["Objects"] )
                {
                    SpawnGameObject( (JObject)objData );
                }

                foreach( var objData in json["Objects"] )
                {
                    SetDataGameObject( (GameObject)identObject[(Guid)objData["$id"]], (JObject)objData );
                }
            }
            finally
            {
                identGuid.Clear();
                identObject.Clear();
            }
        }

        public static JArray SaveSceneObjects()
        {
            // this is important. without this, it won't serialize at the 2nd attempt.
            identGuid = new Dictionary<object, Guid>();

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>(); //find all the persistent objects in the level

            JArray dataJson = new JArray();
            foreach( var obj in allObjects )
            {
                if( SerializationHelper.ShouldSerialize( obj ) )
                {
                    try
                    {
                        JObject data = GetDataGameObject( obj.gameObject );
                        dataJson.Add( data );
                    }
                    catch( Exception ex )
                    {
                        Debug.LogException( ex );
                    }
                }
            }

            return dataJson;
        }

        //
        //
        //

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataGameObject( GameObject obj )
        {
            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            Guid guid = Guid.NewGuid();
            identGuid.Add( obj, guid );

            string prefabPath = null;
            PersistentGameObject pgo = obj.GetComponent<PersistentGameObject>();
            if( pgo != null )
            {
                prefabPath = pgo.PrefabPath;
            }
            else
            {
                prefabPath = obj.GetPrefabPath();
            }

            JObject full = new JObject()
            {
                { "$id", guid },
                { "Prefab", prefabPath },
                { "Name", obj.name },
                { "Transform", transformData },
                { "Components", componentData },
            };

            return full;
        }

        public static void SpawnGameObject( JObject data )
        {
            string prefabPath = (string)data["Prefab"];
            if( prefabPath == null ) 
            {
                throw new Exception( "Prefab Path was null, can't spawn" );
            }
            Guid id = (Guid)data["$id"];

            GameObject gameObject = PersistentGameObject.Instantiate( AssetManager.GetPrefab( prefabPath ) );

            identGuid.Add( gameObject, id );
            identObject.Add( id, gameObject );
        }

        /// <summary>
        /// Applies the saved data to this object.
        /// </summary>
        /// <param name="data">The data. It's supposed to have come from the same object, saved earlier.</param>
        public static void SetDataGameObject( GameObject obj, JObject data )
        {
            obj.name = (string)data["Name"];

            JObject transformData = (JObject)data["Transform"];

            JToken componentData = data["Components"];

            obj.transform.SetData( transformData );
            SetComponentData( obj, componentData );
        }

        /// <summary>
        /// Gets the data for every <see cref="ISerializedComponent"/> component on this object.
        /// </summary>
        public static JToken GetComponentData( GameObject obj )
        {
            ISerializedComponent[] serializedComponents = obj.GetComponents<ISerializedComponent>();

            var componentsGroupedByType = serializedComponents.GroupBy( c => c.GetType() );

            JObject allComps = new JObject();

            if( obj.name == "Player" )
            {

            }

            // Save the components grouped by their type, and for each type, preserve their ordering (order within the type will matter, but each type can be rearranged).
            foreach( var group in componentsGroupedByType )
            {
                JArray compData = new JArray();

                foreach( var sc in group )
                {
                    try
                    {
                        JObject data = sc.GetData();
                        compData.Add( data );
                    }
                    catch( Exception ex )
                    {
                        Debug.LogException( ex );
                    }
                }

                allComps.Add( group.Key.FullName, compData );
            }

            return allComps;
        }

        /// <summary>
        /// Sets the data for every <see cref="ISerializedComponent"/> component on this object.
        /// </summary>
        public static void SetComponentData( GameObject obj, JToken data )
        {
            ISerializedComponent[] serializedComponents = obj.GetComponents<ISerializedComponent>();

            var componentsGroupedByType = serializedComponents.GroupBy( c => c.GetType() );

            foreach( var group in componentsGroupedByType )
            {
                string componentType = group.Key.FullName;
                JArray compData = (JArray)data[componentType];

                int i = 0;
                foreach( var sc in group )
                {
                    try
                    {
                        JObject d = (JObject)compData[i];
                        sc.SetData( d );
                    }
                    catch( Exception ex )
                    {
                        Debug.LogException( ex );
                    }
                    i++;
                }
            }
        }
    }
}