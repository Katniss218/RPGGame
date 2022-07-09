using RPGGame.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGGame.Mobs
{
    [RequireComponent( typeof( MobTargetTracking ) )]
    [DisallowMultipleComponent]
    public class MobAI : MonoBehaviour
    {
        public float ViewRange = 10.0f;

        [SerializeField] private float updateFrequency = 0.5f;

        private MobTargetTracking enemyMovement;

        private float lastPathUpdateTimestamp;
        private float timeSinceLastPathUpdate => Time.time - lastPathUpdateTimestamp;

        void Awake()
        {
            enemyMovement = this.GetComponent<MobTargetTracking>();
        }

        void Update()
        {
            HandleUpdateTarget();
        }

        private void HandleUpdateTarget()
        {
            if( timeSinceLastPathUpdate < updateFrequency )
            {
                return;
            }

            // Stop tracking any targets that are out of range.
            if( enemyMovement.Target != null && Vector3.Distance( this.transform.position, enemyMovement.Target.position ) > ViewRange )
            {
                enemyMovement.Target = null;
            }

            if( enemyMovement.Target == null )
            {
                enemyMovement.Target = TryFindClosestTarget();
            }

            lastPathUpdateTimestamp = Time.time;
        }

        /// <summary>
        /// Checks whether or not a specified object is considered to be an enemy (is targetable).
        /// </summary>
        private bool IsEnemy( Transform obj )
        {
            return obj.GetComponent<PlayerMovementController>() != null;
        }

        private Transform TryFindClosestTarget()
        {
            Collider[] colliders = Physics.OverlapSphere( this.transform.position, ViewRange );

            (Transform t, float d) closestEnemy = (null, float.PositiveInfinity);

            foreach( var collider in colliders )
            {
                Transform colliderTransform = collider.transform;

                if( IsEnemy( colliderTransform ) )
                {
                    float distance = Vector3.Distance( this.transform.position, colliderTransform.position );
                    if( distance < closestEnemy.d )
                    {
                        closestEnemy = (colliderTransform, distance);
                    }
                }
            }

            return closestEnemy.t;
        }
    }
}