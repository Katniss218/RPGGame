using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Mobs
{
    public class MobSpawner : MonoBehaviour
    {
        public GameObject SpawnedObject;

        void Start()
        {
            SpawnMob( SpawnedObject, this.transform );
        }

        void OnDrawGizmos()
        {
            // Draw a yellow sphere at the transform's position
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere( transform.position, 0.5f );
        }

        public static void SpawnMob( GameObject obj, Transform spawner )
        {
            GameObject go = Instantiate( obj, spawner.position, spawner.rotation );

            HealthHandler healthHandler = go.GetComponent<HealthHandler>();

            GameObject hud = Instantiate( Main.MobHud, Main.MobHudCanvas.transform );
            FollowObjectUI follower = hud.GetComponent<FollowObjectUI>();
            follower.TrackedObject = go.transform;
            follower.WorldOffset = new Vector3( 0, 2, 0 ); // in the future, change depending on the size of the monster.

            HealthbarUI healthbar = hud.GetComponent<HealthbarUI>();
            healthHandler.onHealthChange.AddListener( healthbar.OnHealthOrMaxHealthChange );
            healthHandler.onMaxHealthChange.AddListener( healthbar.OnHealthOrMaxHealthChange );
            healthHandler.onDeath.AddListener( healthbar.OnDeath );
        }
    }
}