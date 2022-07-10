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
                Debug.Log( "Camera Controller - FollowTarget was null." ); // this happens when the scene is loaded and the method is called before the field can be set.
                return;
            }

            this.transform.position = FollowTarget.position;
        }
    }
}