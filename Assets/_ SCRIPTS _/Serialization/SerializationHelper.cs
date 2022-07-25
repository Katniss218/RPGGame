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
    public enum ReferenceType
    {
        ID,
        ASSET
    }

    /// <summary>
    /// A helper class converting Unity types to JSON and the other way around.
    /// </summary>
    public static class SerializationHelper
    {
        // Should-serialize stuff

        /// <summary>
        /// This tag indicates that the object should not be serialized.
        /// </summary>
        public static string TAG_NO_SERIALIZATION = "NonSerialized";

        /// <summary>
        /// Checks whether a given object should be serialized or not.
        /// </summary>
        public static bool ShouldSerialize( GameObject obj )
        {
            return obj.transform.parent == null
                && obj.tag != TAG_NO_SERIALIZATION;
        }

        // Reference path stuff.

        const char REF = '$';
        const char REF_SEPARATOR = ';';
        const string ID = "id";
        const string ASSET = "asset";

        public static string ToReferenceString( ReferenceType type, string assetID )
        {
            if( assetID.Contains( REF_SEPARATOR ) )
            {
                throw new ArgumentException( $"The string can't contain the '{REF_SEPARATOR}' char." );
            }

            return type switch
            {
                ReferenceType.ID => $"{REF}{ID}{REF_SEPARATOR}{assetID}",
                ReferenceType.ASSET => $"{REF}{ASSET}{REF_SEPARATOR}{assetID}",
                _ => throw new Exception( $"Unknown reference type '{type}'." )
            };
        }

        public static (ReferenceType type, string assetID) ToAssetID( string refStr )
        {
            string start;

            start = $"{REF}{ID}{REF_SEPARATOR}";
            if( refStr.StartsWith( start ) )
            {
                return (ReferenceType.ID, refStr.Substring( start.Length ));
            }
            start = $"{REF}{ASSET}{REF_SEPARATOR}";
            if( refStr.StartsWith( start ) )
            {
                return (ReferenceType.ASSET, refStr.Substring( start.Length ));
            }

            throw new ArgumentException( $"A reference string must follow the format '{REF}<ref_type>{REF_SEPARATOR}<string>'." );
        }

        //
        //      Unity object serialization stuff.
        //

        public static GameObject SpawnAndRegisterGameObject( JObject data )
        {
            string prefabPath = (string)data["Prefab"];
            if( prefabPath == null )
            {
                throw new Exception( "Prefab Path was null, can't spawn" );
            }

            GameObject gameObject = RPGObject.Instantiate( prefabPath, "<not_assigned_yet>" );

            SerializationManager.RegisterObject( (Guid)data["$id"], gameObject );

            return gameObject;
        }

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataGameObject( GameObject obj )
        {
            RPGObject rpgObject = obj.GetComponent<RPGObject>();

            string prefabPath;

            if( rpgObject == null )
            {
#if UNITY_EDITOR
                prefabPath = RPGObject.GetLoadablePrefabPath( obj );
#else
                throw new Exception( "Can't serialize a non-RPGObject." );
#endif
            }
            else
            {
                prefabPath = rpgObject.PrefabPath;
            }

            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            Guid objectId = Guid.NewGuid();
            SerializationManager.RegisterObject( objectId, obj );

            JObject full = new JObject()
            {
                { "$id", objectId },
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