using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RPGGame.Serialization
{
    /// <summary>
    /// A helper class converting Unity types to JSON and the other way around.
    /// </summary>
    public static class SerializationHelper
    {
        /// <summary>
        /// This tag indicates that the object should not be serialized.
        /// </summary>
        public static string TAG_NO_SERIALIZATION = "NonSerialized";

        /// <summary>
        /// Checks whether a given object should be auto-serialized or not.
        /// </summary>
        public static bool ShouldAutoSerialize( GameObject obj )
        {
            return obj.transform.parent == null
                && obj.tag != TAG_NO_SERIALIZATION;
        }

        //
        //      Unity object serialization stuff.
        //

        public static (RPGObject, Guid) SpawnRpgObject( JObject data )
        {
            string prefabPath = (string)data["Prefab"];
            if( prefabPath == null )
            {
                throw new Exception( "Prefab Path was null, can't spawn" );
            }

            (RPGObject, Guid) pair = RPGObject.Instantiate( prefabPath, "<not_assigned_yet>", (Guid)data["$id"] );

            return pair;
        }

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataRpgObject( RPGObject obj )
        {
            Guid objGuid;
            string prefabPath;

            RPGObject rpgObject = obj.GetComponent<RPGObject>();

            if( !Application.isPlaying ) // Serialize within the editor (save scene button).
            {
#if UNITY_EDITOR
                prefabPath = RPGObject.GetLoadablePrefabPath( obj.gameObject );
                rpgObject.guid = Guid.NewGuid();
#else
                throw new Exception( "Can't serialize a non-RPGObject outside of the editor." );
#endif
            }
            else // Serialize within the game.
            {
                objGuid = RPGObject.Get( rpgObject );
                prefabPath = rpgObject.PrefabPath;
            }

            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            JObject full = new JObject()
            {
                { "$id", rpgObject.guid },
                { "Prefab", prefabPath },
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
        public static void SetDataGameObject( RPGObject obj, JObject data )
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
        public static JToken GetComponentData( RPGObject obj )
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
        public static void SetComponentData( RPGObject obj, JToken data )
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

        //-------------------------------------
        //-------------------------------------
        //-------------------------------------
        //-------------------------------------
        //-------------------------------------

        /*

        {
            "$type": "<AssemblyQualifiedName>",

            // ... Other data.
        }

        */

        /// <summary>
        /// Deserializes an object while preserving the original derived type it was serializes with.
        /// </summary>
        public static T DeserializeTyped<T>( JObject json ) where T : ISerializedComponent
        {
            string typeS = (string)json["$type"];

            Type type = Type.GetType( typeS ); // type must be serialized as 'Type.AssemblyQualifiedName'.

            if( !typeof( T ).IsAssignableFrom( type ) )
            {
                throw new Exception( $"The serialized type is not a '{typeof( T ).AssemblyQualifiedName}' or a class derived from it." );
            }

#warning TODO - this is slow, there are other (several tens of times faster methods) methods that can replace it.
            T o = (T)Activator.CreateInstance( type );

            o.SetData( json );

            return o;
        }

        /// <summary>
        /// Serializes the object while preserving its derived type.
        /// </summary>
        public static JObject SerializeTyped<T>( T obj ) where T : ISerializedComponent
        {
            Type type = obj.GetType();

            string typeS = type.AssemblyQualifiedName;

            JObject json = obj.GetData();

            json.Add( "$type", typeS );

            return json;
        }
    }
}