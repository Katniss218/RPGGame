using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    [DisallowMultipleComponent]
    public class SingletonMonoBehaviour<T> : MonoBehaviour where T : Component
    {
        private static T __instance;

        /// <summary>
        /// Returns the typed instance of this singleton component. A O(1) operation once the instance is set.
        /// </summary>
        public static T Instance
        {
            get
            {
                if( __instance == null )
                {
                    T[] temp = FindObjectsOfType<T>();

                    if( temp.Length < 1 )
                    {
                        throw new System.Exception( $"Tried accessing a singleton component '{typeof( T )}', but none exist." );
                    }
                    if( temp.Length > 1 )
                    {
                        throw new System.Exception( $"Duplicated singleton component '{typeof( T )}'." );
                    }

                    __instance = temp[0];
                }

                return __instance;
            }
        }
    }
}