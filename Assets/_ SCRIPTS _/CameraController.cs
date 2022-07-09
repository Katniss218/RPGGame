using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    public class CameraController : MonoBehaviour
    {
        public Transform FollowTarget;

        [SerializeField] private new Camera camera;
        public Camera Camera { get => camera; }

        void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            if( FollowTarget == null )
            {
                Debug.Log( "FollowTarget was null." );
                return;
            }

            this.transform.position = FollowTarget.position;
        }
    }
}