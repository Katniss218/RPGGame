using RPGGame.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace RPGGame
{
    [DisallowMultipleComponent]
    [ExecuteAlways]
    public sealed class RPGObject : MonoBehaviour
    {
        [field: SerializeField] 
        public string PrefabPath { get; private set; }

        private void Awake()
        {
#if UNITY_EDITOR
            if( !Application.isPlaying )
            {
                PrefabPath = GetLoadablePrefabPath( this.gameObject );
            }
#endif
        }

        void Start()
        {
#if UNITY_EDITOR
            if( !Application.isPlaying )
            {
                return;
            }
#endif
            if( PrefabPath == null )
            {
                throw new InvalidOperationException( $"The PrefabPath for object '{this.gameObject.name}' wasn't set. It must've been spawned incorrectly. Use the 'RPGObject.Instantiate' method." );
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

            if( string.IsNullOrEmpty(rawPath) )
            {
                throw new Exception( $"Encountered RPGObject '{obj.name}' is not a prefab." );
            }
            if( string.IsNullOrEmpty( rawPath ) )
            {
                return null;
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

        public static GameObject Instantiate( string prefabPath, string name, Transform parent = null, Vector3? localPos = null, Quaternion? localRot = null )
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
                obj = clone.AddComponent<RPGObject>();
            }

            obj.PrefabPath = prefabPath;

            return clone;
        }
    }
}