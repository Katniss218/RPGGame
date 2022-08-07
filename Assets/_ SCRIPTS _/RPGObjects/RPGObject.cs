using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RPGGame
{
    /// <summary>
    /// RPGObjects are basically the non-static serialized scene objects.
    /// </summary>
    /// <remarks>
    /// RPGObjects are not supposed to be persistent, they just store the data during the play.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class RPGObject : MonoBehaviour
    {
        /// <summary>
        /// Stores all loaded RPGObjects at runtime.
        /// </summary>
        static Dictionary<Guid, RPGObject> allRpgObjects = new Dictionary<Guid, RPGObject>();

        /// <summary>
        /// The 'Resources/Prefabs/' path of this particular RPGObject. 'Set' only for editor serialization.
        /// </summary>
        [field: NonSerialized]
        public string PrefabPath { get; internal set; } = null;

        /// <summary>
        /// The globally unique identifier of this particular RPGObject. 'Set' only for editor serialization.
        /// </summary>
        [field: NonSerialized]
        public Guid guid { get; internal set; } = default;

        /// <summary>
        /// Returns the RPGObject corresponding to the given identifier, O(1).
        /// </summary>
        public static RPGObject Get( Guid guid )
        {
            if( guid == Guid.Empty )
            {
                throw new ArgumentNullException( "Guid must be set" );
            }
            return allRpgObjects[guid];
        }

        /// <summary>
        /// Returns the identifier corresponding to the given RPGObject, O(1).
        /// </summary>
        public static Guid Get( RPGObject obj )
        {
            if( obj == null )
            {
                throw new ArgumentNullException( "Obj can't be null" );
            }
            return obj.guid;
        }

        static void RegisterNew( RPGObject obj )
        {
            if( obj.guid == Guid.Empty )
            {
                obj.guid = Guid.NewGuid();
            }

            allRpgObjects[obj.guid] = obj;
        }

        void Start()
        {
            if( PrefabPath == null )
            {
                throw new InvalidOperationException( $"The PrefabPath for object '{this.gameObject.name}' wasn't set. It must've been spawned incorrectly. Use 'RPGObject.Instantiate' methods." );
            }
            if( guid == default )
            {
                throw new InvalidOperationException( $"The Guid for object '{this.gameObject.name}' wasn't set. It must've been spawned incorrectly. Use 'RPGObject.Instantiate' methods." );
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Gets the path for a given prefab object.
        /// </summary>
        /// <returns>The path, relative to 'Assets/Resources/'. Null if object is not a prefab.</returns>
        public static string GetLoadablePrefabPath( GameObject obj )
        {
            string rawPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot( obj );

            if( string.IsNullOrEmpty( rawPath ) )
            {
                throw new Exception( $"Encountered RPGObject '{obj.name}' is not a prefab." );
            }

            if( !rawPath.StartsWith( "Assets/Resources/" ) )
            {
                throw new Exception( $"Encountered RPGObject '{rawPath}' is not loadable at runtime. Please put it in 'Assets/Resources/'." );
            }

            string prefabPath = Path.GetRelativePath( "Assets/Resources", rawPath );
            prefabPath = Path.ChangeExtension( prefabPath, null );

            return prefabPath;
        }
#endif

        /// <summary>
        /// Use this to create an RPGObject.
        /// </summary>
        public static (RPGObject, Guid) Instantiate( string prefabPath, string name, Guid guid = default, Transform parent = null, Vector3? localPos = null, Quaternion? localRot = null )
        {
            GameObject prefab = AssetManager.Prefabs.Get( prefabPath );

            GameObject clone = Instantiate( prefab, parent );
            clone.name = name;

            if( localPos != null )
            {
                clone.transform.localPosition = localPos.Value;
            }

            if( localRot != null )
            {
                clone.transform.localRotation = localRot.Value;
            }

            RPGObject obj = clone.GetComponent<RPGObject>();

            if( obj == null )
            {
                throw new Exception( $"Tried to spawn a prefab '{prefabPath}' that is not an RPGObject." );
            }

            obj.PrefabPath = prefabPath;
            obj.guid = guid;

            RegisterNew( obj );

            return (obj, obj.guid);
        }

        /// <summary>
        /// Use this to destroy an RPGObject.
        /// </summary>
        public static void Destroy( RPGObject obj )
        {
            if( obj == null )
            {
                throw new Exception( "Object can't be null." );
            }
            if( obj.guid == Guid.Empty )
            {
                throw new Exception( "Object must have its guid set." );
            }

            Object.Destroy( obj.gameObject );
            allRpgObjects.Remove( obj.guid );
        }

        void OnDestroy()
        {
            // scene changes and accidental destroys (you're not supposed to destroy the objects with the Unity's destroy).
            if( allRpgObjects.ContainsKey( this.guid ) )
            {
                allRpgObjects.Remove( this.guid );
            }
        }
    }
}