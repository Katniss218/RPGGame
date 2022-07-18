using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public static Dictionary<object, Guid> identifyableObjects = new Dictionary<object, Guid>();

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

        public static JArray SaveSceneObjects()
        {
            // this is important. without this, it won't serialize at the 2nd attempt.
            identifyableObjects = new Dictionary<object, Guid>();

            GameObject[] persist = UnityEngine.Object.FindObjectsOfType<GameObject>(); //find all the persistent objects in the level

            JArray dataJson = new JArray();
            foreach( var persistent in persist )
            {
                // only serialize root objects.
                if( persistent.transform.parent != null )
                {
                    continue;
                }
                try
                {
                    JObject data = GetDataGameObject( persistent.gameObject );
                    dataJson.Add( data );
                }
                catch( Exception ex )
                {
                    Debug.LogException( ex );
                }
            }

            return dataJson;
        }

        //

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataGameObject( GameObject _obj )
        {
            GameObject obj = _obj.gameObject;

            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            Guid guid = Guid.NewGuid();
            identifyableObjects.Add( _obj, guid );

            JObject full = new JObject()
            {
                { "$id", guid.ToString("D") },
                { "Prefab", obj.GetPrefabPath() },
                { "Name", obj.name },
                { "Transform", transformData },
                { "Components", componentData },
            };

            return full;
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