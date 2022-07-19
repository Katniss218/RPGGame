using RPGGame.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Serialization
{
    public class PersistentGameObject : MonoBehaviour
    {
        public string PrefabPath { get; private set; }

        public static GameObject Instantiate( string prefabPath )
        {
            GameObject gameObject = Instantiate( AssetManager.GetPrefab( prefabPath ), null );

            PersistentGameObject pgo = gameObject.AddComponent<PersistentGameObject>();
            pgo.PrefabPath = prefabPath;

            return gameObject;
        }
    }
}