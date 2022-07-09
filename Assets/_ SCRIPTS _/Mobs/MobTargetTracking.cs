using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGGame.Mobs
{
    [RequireComponent( typeof( NavMeshAgent ) )]
    [DisallowMultipleComponent]
    public class MobTargetTracking : MonoBehaviour
    {
        public Transform Target { get; set; } = null;

        [SerializeField] private float pathUpdateFrequency = 0.5f;

        private NavMeshAgent navMeshAgent;

        private Vector3 lastTargetPosition;

        private float lastPathUpdateTimestamp;
        private float timeSinceLastUpdate => Time.time - lastPathUpdateTimestamp;

        private void Awake()
        {
            navMeshAgent = this.GetComponent<NavMeshAgent>();
        }

        void Update()
        {
            if( timeSinceLastUpdate < pathUpdateFrequency )
            {
                return;
            }

            if( Target == null )
            {
                return;
            }

            HandleMovementToTarget();

            lastTargetPosition = Target.position;
        }

        private void HandleMovementToTarget()
        {
            // If the target has moved in this frame, or
            // If the last calculated path leads to its previous position (it has since stopped, but the path is out of date).
            if( this.Target.position != lastTargetPosition || this.navMeshAgent.pathEndPosition != lastTargetPosition )
            {
                this.navMeshAgent.SetDestination( Target.position );
                lastPathUpdateTimestamp = Time.time;
            }
        }
    }
}