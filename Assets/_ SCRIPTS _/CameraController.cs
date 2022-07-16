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
                // this happens when the scene is loaded and the method is called before the field can be set.
                // it also happens when we're just waiting e.g. in the main menu.

                return;
            }

            this.transform.position = FollowTarget.position;
        }
    }
}