using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    /// <summary>
    /// Controls the camera
    /// </summary>
    public class CameraController : SingletonMonoBehaviour<CameraController>
    {
        /// <summary>
        /// The target that this camera controller will follow.
        /// </summary>
        [field: SerializeField]
        public Transform FollowTarget { get; set; }

        /// <summary>
        /// Returns the main camera.
        /// </summary>
        public static Camera Camera { get => Instance.camera; }

        [SerializeField] private new Camera camera;

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