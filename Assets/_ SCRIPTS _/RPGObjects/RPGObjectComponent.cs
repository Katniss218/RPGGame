using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    [RequireComponent( typeof( RPGObject ) )]
    public class RPGObjectComponent : MonoBehaviour
    {
        private RPGObject rpgObject;

        public RPGObject RpgObject
        {
            get
            {
                if( rpgObject == null )
                {
                    rpgObject = this.GetComponent<RPGObject>();
                }
                return rpgObject;
            }
        }
    }
}