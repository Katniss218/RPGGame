using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPGGame
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent navMeshAgent;

        public float TargetUpdateFrequency;

        public Transform Target;

        private Vector3 lastTargetPosition;
        private float lastPathUpdateTimestamp;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            HandleMovementToTarget();

            lastTargetPosition = Target.position;
        }

        private void HandleMovementToTarget()
        {
            if( Time.time < lastPathUpdateTimestamp + TargetUpdateFrequency )
            {
                return;
            }
            if( this.Target.position != lastTargetPosition || this.navMeshAgent.pathEndPosition != lastTargetPosition )
            {
                this.navMeshAgent.SetDestination( Target.position );
                lastPathUpdateTimestamp = Time.time;
            }
        }
    }
}