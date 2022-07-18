using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RPGGame.ObjectCreation
{
    //[ExecuteInEditMode]
    //[DisallowMultipleComponent]
    //public class Persistent : MonoBehaviour, ISerializationCallbackReceiver
    public static class Persistent
    {/*
        public static Dictionary<Guid, GameObject> AllPersistentObjects = new Dictionary<Guid, GameObject>();

        [NonSerialized] public string PrefabPath = null;

        /// <summary>
        /// True if the object was spawned and was not part of the scene.
        /// </summary>
        public bool IsSpawned => this.PrefabPath != null;

        public bool HasGuid => guid != Guid.Empty;

        Guid guid = Guid.Empty;

        [SerializeField] [HideInInspector] byte[] serializedGuid;

        public Guid GetGuid()
        {
            // Never return an invalid GUID
            if( guid == Guid.Empty && serializedGuid != null && serializedGuid.Length == 16 )
            {
                guid = new Guid( serializedGuid );
            }

            return guid;
        }

        // When de-serializing or creating this component, we want to either restore our serialized GUID
        // or create a new one.
        void CreateOrRestoreGuid()
        {
            // if our serialized data is invalid, then we are a new object and need a new GUID
            if( serializedGuid == null || serializedGuid.Length != 16 )
            {
#if UNITY_EDITOR
                // if in editor, make sure we aren't a prefab of some kind
                if( IsAssetOnDisk() )
                {
                    return;
                }
#endif
                guid = Guid.NewGuid();
                serializedGuid = guid.ToByteArray();

#if UNITY_EDITOR
                // If we are creating a new GUID for a prefab instance of a prefab, but we have somehow lost our prefab connection
                // force a save of the modified prefab instance properties
                if( PrefabUtility.IsPartOfNonAssetPrefabInstance( this ) )
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications( this );
                }
#endif
            }
            // otherwise, we should set our system guid to our serialized guid
            else if( guid == Guid.Empty )
            {
                guid = new Guid( serializedGuid );
            }

            // register with the GUID Manager so that other components can access this
            if( guid != Guid.Empty )
            {
                if( AllPersistentObjects.ContainsKey( this.guid ) )
                {
#warning TODO - when duplicating, it copies the GUID of the object, so we have 2 objects with the same guid.
                    // if registration fails, we probably have a duplicate or invalid GUID, get us a new one.
                    serializedGuid = null;
                    guid = Guid.Empty;
                    CreateOrRestoreGuid();
                }
                else
                {
                    AllPersistentObjects.Add( guid, this.gameObject );
                }
            }
        }

        // This is needed to work around the OnValidate() being called twice when duplicating an object (we'd get a double Guid in the 'AllPersistentObjects' otherwise).
        [NonSerialized] bool alreadySet;

        void OnValidate()
        {
#if UNITY_EDITOR
            // similar to on Serialize, but gets called on Copying a Component or Applying a Prefab
            // at a time that lets us detect what we are
            if( IsAssetOnDisk() )
            {
                serializedGuid = null;
                guid = Guid.Empty;
            }
            else
#endif
            {
                if( alreadySet )
                {
                    return;
                }
                CreateOrRestoreGuid();
                alreadySet = true;
            }
        }

#if UNITY_EDITOR
        private bool IsEditingInPrefabMode()
        {
            if( EditorUtility.IsPersistent( this ) )
            {
                // if the game object is stored on disk, it is a prefab of some kind, despite not returning true for IsPartOfPrefabAsset =/
                return true;
            }
            else
            {
                // If the GameObject is not persistent let's determine which stage we are in first because getting Prefab info depends on it
                var mainStage = StageUtility.GetMainStageHandle();
                var currentStage = StageUtility.GetStageHandle( gameObject );
                if( currentStage != mainStage )
                {
                    var prefabStage = PrefabStageUtility.GetPrefabStage( gameObject );
                    if( prefabStage != null )
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// True if this object right now is a prefab in the prefab scene.
        /// </summary>
        private bool IsAssetOnDisk()
        {
            return PrefabUtility.IsPartOfPrefabAsset( this ) || IsEditingInPrefabMode();
        }
#endif

        // We cannot allow a GUID to be saved into a prefab, and we need to convert to byte[]
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR
            // This lets us detect if we are a prefab instance or a prefab asset.
            // A prefab asset cannot contain a GUID since it would then be duplicated when instanced.
            if( IsAssetOnDisk() )
            {
                serializedGuid = null;
                guid = Guid.Empty;
            }
            else
#endif
            {
                if( guid != Guid.Empty )
                {
                    serializedGuid = guid.ToByteArray();
                }
            }
        }

        // On load, we can go head a restore our system guid for later use
        public void OnAfterDeserialize()
        {
            if( serializedGuid != null && serializedGuid.Length == 16 )
            {
                guid = new Guid( serializedGuid );
            }
        }

        // let the manager know we are gone, so other objects no longer find this
        public void OnDestroy()
        {
            AllPersistentObjects.Remove( guid );
        }
        */
        //
        //
        //

        public static GameObject InstantiatePersistent( string prefabPath, string name, Guid? guid, Vector3 position, Quaternion rotation )
        {
            if( guid == null )
            {
                guid = Guid.NewGuid();
            }

            GameObject obj = GameObject.Instantiate( AssetManager.GetPrefab( prefabPath ), position, rotation );
            obj.name = name;
            /*
            Persistent persistent = obj.GetComponent<Persistent>();
            if( persistent == null )
            {
                persistent = obj.AddComponent<Persistent>();
            }
            persistent.guid = guid.Value;
            persistent.serializedGuid = persistent.guid.ToByteArray();
            persistent.PrefabPath = prefabPath;
            */
            return obj;
        }
    }
}