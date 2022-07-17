using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.ObjectCreation;
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
        public static JObject SavePersistentObjects()
        {
            Persistent[] persist = UnityEngine.Object.FindObjectsOfType<Persistent>(); //find all the persistent objects in the level

            JObject dataJson = new JObject();
            foreach( var persistent in persist )
            {
                try
                {
                    JObject data = GetDataGameObject( persistent );
                    dataJson.Add( persistent.guid, data );
                }
                catch( Exception ex )
                {
                    Debug.LogException( ex );
                }
            }

            return dataJson;
        }

        public static void LoadPersistentObjects( JObject data )
        {
            Persistent[] persist = UnityEngine.Object.FindObjectsOfType<Persistent>();

            foreach( var (guid, _objData) in data )
            {
                JObject objData = (JObject)_objData;

                try
                {

                    Persistent persistent;
                    string prefabPath = (string)objData["PrefabPath"];
                    // if the prefab path is not null, spawn it.
                    if( prefabPath != null )
                    {
                        persistent = Persistent.InstantiatePersistent( prefabPath, default, guid, default, default ).GetComponent<Persistent>();
                    }
                    else
                    {
                        persistent = persist.First( p => p.guid == guid );
                    }

                    SetDataGameObject( persistent, objData );
                }
                catch( Exception ex )
                {
                    Debug.LogException( ex );
                }
            }
        }

        //

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataGameObject( Persistent _obj )
        {
            GameObject obj = _obj.gameObject;

            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            JObject full = new JObject()
            {
                { "PrefabPath", _obj.PrefabPath },
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
        public static void SetDataGameObject( Persistent _obj, JObject data )
        {
            GameObject obj = _obj.gameObject;

            obj.name = (string)data["Name"];

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