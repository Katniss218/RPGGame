using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    [DisallowMultipleComponent]
    public class Follower : MonoBehaviour
    {
        public Transform Target;

        void Update()
        {
            if( Target != null )
            {
                this.transform.SetPositionAndRotation( Target.transform.position, Target.transform.rotation );
            }
        }
    }
}