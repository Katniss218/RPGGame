using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGGame.ObjectCreation
{
    [ExecuteInEditMode]
    public class Persistent : MonoBehaviour
    {
        [NonSerialized] public string PrefabPath = null;

#warning TODO - eventually, every persistent object should be spawned from a prefab on load.
        /// <summary>
        /// True if the object was spawned and was not part of the scene.
        /// </summary>
        public bool IsSpawned { get; private set; }

        public string guid;

#if UNITY_EDITOR

        /// <summary>
        /// Create a new unique ID for this object when it's created
        /// </summary>
        private void Awake()
        {
            if( Application.platform != RuntimePlatform.WindowsEditor )
            {
                guid = Guid.NewGuid().ToString();
                PrefabUtility.RecordPrefabInstancePropertyModifications( this );
            }
        }

        /// <summary>
        /// This is only needed if you are adding this script to prefabs that already have instances of in your scenes
        /// - This will update any object that doesn't already have a guid with one.
        /// </summary>
        private void Update()
        {
            if( string.IsNullOrEmpty( guid ) )
            {
                guid = Guid.NewGuid().ToString();
                PrefabUtility.RecordPrefabInstancePropertyModifications( this );
            }
        }
#endif

        public static GameObject InstantiatePersistent( string prefabPath, string name, string guid, Vector3 position, Quaternion rotation )
        {
            GameObject obj = Instantiate( AssetManager.GetPrefab( prefabPath ), position, rotation );
            obj.name = name;

            Persistent persistent = obj.GetComponent<Persistent>();
            if( persistent == null )
            {
                persistent = obj.AddComponent<Persistent>();
            }
            persistent.guid = string.IsNullOrEmpty( guid ) ? Guid.NewGuid().ToString() : guid;
            persistent.PrefabPath = prefabPath;
            persistent.IsSpawned = true;

            return obj;
        }
    }
}