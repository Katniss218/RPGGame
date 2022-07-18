using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGGame
{
    /// <summary>
    /// A helper class converting Unity types to JSON and the other way around.
    /// </summary>
    public static class SerializationHelper
    {
        // Vector2

        public static JToken ToJson( this Vector2 obj )
        {
            return new JArray( obj.x, obj.y );
        }

        public static Vector2 ToVector2( this JToken json )
        {
            return new Vector2( (float)json[0], (float)json[1] );
        }

        // Vector2Int

        public static JToken ToJson( this Vector2Int obj )
        {
            return new JArray( obj.x, obj.y );
        }

        public static Vector2Int ToVector2Int( this JToken json )
        {
            return new Vector2Int( (int)json[0], (int)json[1] );
        }

        // Vector3

        public static JToken ToJson( this Vector3 obj )
        {
            return new JArray( obj.x, obj.y, obj.z );
        }

        public static Vector3 ToVector3( this JToken json )
        {
            return new Vector3( (float)json[0], (float)json[1], (float)json[2] );
        }

        // Vector3Int

        public static JToken ToJson( this Vector3Int obj )
        {
            return new JArray( obj.x, obj.y, obj.z );
        }

        public static Vector3Int ToVector3Int( this JToken json )
        {
            return new Vector3Int( (int)json[0], (int)json[1], (int)json[2] );
        }

        // Quaternion

        public static JToken ToJson( this Quaternion obj )
        {
            return new JArray( obj.x, obj.y, obj.z, obj.w );
        }

        public static Quaternion ToQuaternion( this JToken json )
        {
            return new Quaternion( (float)json[0], (float)json[1], (float)json[2], (float)json[3] );
        }

        // 

        //

        public static string GetPrefabPath( this GameObject obj )
        {
            // Returns a 'Prefabs/foo/bar/baz' path, where 'baz' is the filename.
            // Null if object is not a prefab.

            string rawPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot( obj );

            if( string.IsNullOrEmpty( rawPath ) )
            {
                return null;
            }

            if( !rawPath.StartsWith( "Assets/Resources/" ) )
            {
                throw new Exception( $"Encountered a prefab that is not loadable. Please put it in Assets/Resources/... - '{rawPath}'" );
            }

            string prefabPath = System.IO.Path.GetRelativePath( "Assets/Resources", rawPath );
            prefabPath = System.IO.Path.ChangeExtension( prefabPath, null );

            return prefabPath;
        }
    }
}