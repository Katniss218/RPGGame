using System;
using UnityEditor;
using UnityEngine;

namespace PersistentObject
{
    [ExecuteInEditMode]
    public class Persistent : MonoBehaviour
    {
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
    }
}