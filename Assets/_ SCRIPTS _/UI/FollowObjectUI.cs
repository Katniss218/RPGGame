using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class FollowObjectUI : MonoBehaviour
    {
        public Transform TrackedObject;
        public Vector3 WorldOffset;
        public Vector3 ScreenOffset;

        void Start()
        {

        }

        void Update()
        {
            SnapToTracked();
        }

        /// <summary>
        /// Snaps the HUD to it's holder transform.
        /// </summary>
        public void SnapToTracked()
        {
            this.transform.position = Main.Camera.WorldToScreenPoint( TrackedObject.transform.position + WorldOffset ) + ScreenOffset;
        }
    }
}