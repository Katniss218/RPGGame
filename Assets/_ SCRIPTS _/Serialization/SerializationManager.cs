using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersistentObject;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RPGGame.Serialization
{
    public static class SerializationManager
    {
        //

        /// <summary>
        /// Gets the data for all persistent objects in the scene.
        /// </summary>
        public static JObject GetDataForPersistentObjects()
        {
            // returns a "list" of gameobjects keyed with their guid.

            Persistent[] persist = UnityEngine.Object.FindObjectsOfType<Persistent>(); //find all the persistent objects in the level

            JObject json = new JObject();
            foreach( var persistent in persist )
            {
                JObject gameObjectData = GetDataGameObject( persistent.gameObject );

                json.Add( persistent.guid, gameObjectData );
            }

            return json;
        }

        /// <summary>
        /// Sets the data for all persistent objects in the scene.
        /// </summary>
        public static void SetDataForPersistentObjects( JObject data )
        {
            Persistent[] persist = UnityEngine.Object.FindObjectsOfType<Persistent>(); //find all the persistent objects in the level

            foreach( var persistent in persist )
            {
                GameObject go = persistent.gameObject;
                JObject objData = (JObject)data[persistent.guid];

                if( objData == null )
                {
                    Debug.LogError( $"The data for object '{persistent.guid}' ('{go.name}') was missing." );
                    continue;
                }

                SetDataGameObject( go, objData );
            }
        }

        //

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataGameObject( GameObject obj )
        {
            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            JObject full = new JObject()
            {
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
            JObject transformData = (JObject)data["Transform"];

            JToken componentData = data["Components"];

            obj.transform.SetData( transformData );
            SetComponentData( obj, componentData );
        }

        /// <summary>
        /// Gets the data for every <see cref="ISerializedByData"/> component on this object.
        /// </summary>
        public static JToken GetComponentData( GameObject obj )
        {
            ISerializedByData[] serializedComponents = obj.GetComponents<ISerializedByData>();

            var componentsGroupedByType = serializedComponents.GroupBy( c => c.GetType() );

            JObject allComps = new JObject();

            // Save the components grouped by their type, and for each type, preserve their ordering (order within the type will matter, but each type can be rearranged).
            foreach( var group in componentsGroupedByType )
            {
                JArray compData = new JArray();

                foreach( var sc in group )
                {
                    JObject data = sc.GetData();
                    compData.Add( data );
                }

                allComps.Add( group.Key.FullName, compData );
            }

            return allComps;
        }

        /// <summary>
        /// Sets the data for every <see cref="ISerializedByData"/> component on this object.
        /// </summary>
        public static void SetComponentData( GameObject obj, JToken data )
        {
            ISerializedByData[] serializedComponents = obj.GetComponents<ISerializedByData>();

            var componentsGroupedByType = serializedComponents.GroupBy( c => c.GetType() );

            foreach( var group in componentsGroupedByType )
            {
                string componentType = group.Key.FullName;
                JArray compData = (JArray)data[componentType];

                int i = 0;
                foreach( var sc in group )
                {
                    JObject d = (JObject)compData[i];
                    sc.SetData( d );
                    i++;
                }
            }
        }
    }
}