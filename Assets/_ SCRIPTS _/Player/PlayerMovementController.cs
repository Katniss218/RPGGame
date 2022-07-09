using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Player
{
    [DisallowMultipleComponent]
    public class PlayerMovementController : MonoBehaviour
    {
        /// <summary>
        /// Camera pivot to fix the "forward" axis to.
        /// </summary>
        public Transform CameraPivot;

        public float MovementSpeed;

        [SerializeField] private new Rigidbody rigidbody;

        void Update()
        {
            HandleMovement();

            HandleRotation();
        }

        /// <summary>
        /// Get the direction in world space from user input.
        /// </summary>
        private Vector3 GetDirectionFromInput()
        {
            Vector3 offset = Vector3.zero;

            if( Input.GetKey( KeyCode.A ) )
            {
                offset += CameraPivot.TransformDirection( new Vector3( -1, 0, 0 ).normalized );
            }
            else if( Input.GetKey( KeyCode.D ) )
            {
                offset += CameraPivot.TransformDirection( new Vector3( 1, 0, 0 ).normalized );
            }

            if( Input.GetKey( KeyCode.W ) )
            {
                offset += CameraPivot.TransformDirection( new Vector3( 0, 0, 1 ).normalized );
            }
            else if( Input.GetKey( KeyCode.S ) )
            {
                offset += CameraPivot.TransformDirection( new Vector3( 0, 0, -1 ).normalized );
            }

            offset.Normalize();

            return offset;
        }

        private void HandleMovement()
        {
            Vector3 offset = GetDirectionFromInput();

            this.rigidbody.velocity = offset * MovementSpeed;
        }

        private void HandleRotation()
        {
            Ray ray = Main.Camera.ScreenPointToRay( Input.mousePosition );
            if( Physics.Raycast( ray, out RaycastHit hitInfo, float.PositiveInfinity, 1 << 3 ) )
            {
                Vector3 hitPointConstrained = hitInfo.point;
                hitPointConstrained.y = this.transform.position.y;

                Vector3 dir = hitPointConstrained - this.transform.position;
                dir.Normalize();

                this.transform.rotation = Quaternion.LookRotation( dir, Vector3.up );
            }
        }
    }
}