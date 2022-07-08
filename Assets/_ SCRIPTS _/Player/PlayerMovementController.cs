using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        public float Speed;

        /// <summary>
        /// Camera pivot to fix the "forward" axis to.
        /// </summary>
        [SerializeField] private Transform cameraPivot;
        [SerializeField] private new Rigidbody rigidbody;

        void Update()
        {
            HandleMovement();
        }

        /// <summary>
        /// Get the direction in world space from user input.
        /// </summary>
        private Vector3 GetDirectionFromInput()
        {
            Vector3 offset = Vector3.zero;

            if( Input.GetKey( KeyCode.A ) )
            {
                offset += cameraPivot.TransformDirection( new Vector3( -Speed, 0, 0 ).normalized );
            }
            else if( Input.GetKey( KeyCode.D ) )
            {
                offset += cameraPivot.TransformDirection( new Vector3( Speed, 0, 0 ).normalized );
            }

            if( Input.GetKey( KeyCode.W ) )
            {
                offset += cameraPivot.TransformDirection( new Vector3( 0, 0, Speed ).normalized );
            }
            else if( Input.GetKey( KeyCode.S ) )
            {
                offset += cameraPivot.TransformDirection( new Vector3( 0, 0, -Speed ).normalized );
            }

            offset.Normalize();

            return offset;
        }

        private void HandleMovement()
        {
            Vector3 offset = GetDirectionFromInput();

            this.rigidbody.velocity = offset * Speed;
        }
    }
}