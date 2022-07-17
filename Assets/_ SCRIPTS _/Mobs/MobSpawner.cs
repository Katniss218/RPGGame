using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Mobs
{
    public class MobSpawner : MonoBehaviour
    {
        public string PrefabPath;

        void Start()
        {
            Main.SpawnPersistentObject( PrefabPath, "mob", null, this.transform.position, this.transform.rotation );
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere( transform.position, 0.5f );
        }
    }
}