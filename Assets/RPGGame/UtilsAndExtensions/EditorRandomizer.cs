using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    [ExecuteInEditMode]
    public class EditorRandomizer : MonoBehaviour
    {
        public Vector3 RotationMin = new Vector3( 0.0f, -180.0f, 0.0f );
        public Vector3 RotationMax = new Vector3( 0.0f, 180.0f, 0.0f );

        public float ScaleMin = 0.75f;
        public float ScaleMax = 1.25f;

        void Start()
        {
            float rotX = Random.Range( RotationMin.x, RotationMax.x );
            float rotY = Random.Range( RotationMin.y, RotationMax.y );
            float rotZ = Random.Range( RotationMin.z, RotationMax.z );
            this.transform.rotation = Quaternion.Euler( rotX, rotY, rotZ );

            float scale = Random.Range( ScaleMin, ScaleMax );
            this.transform.localScale = new Vector3( scale, scale, scale );
        }
    }
}