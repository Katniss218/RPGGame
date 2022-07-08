using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform pivot;
        [SerializeField] private Transform target;

        void Update()
        {
            HandleMovement();
        }

        private void HandleMovement()
        {
            pivot.position = target.position;
        }
    }
}