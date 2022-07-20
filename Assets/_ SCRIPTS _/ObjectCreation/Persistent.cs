using RPGGame.Assets;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace RPGGame.ObjectCreation
{
    public static class Persistent
    {
        public static GameObject InstantiatePersistent( string prefabPath, string name, Guid? guid, Vector3 position, Quaternion rotation )
        {
            if( guid == null )
            {
                guid = Guid.NewGuid();
            }

            GameObject obj = UnityEngine.Object.Instantiate( AssetManager.Prefabs.Get( prefabPath ), position, rotation );
            obj.name = name;

            return obj;
        }
    }
}