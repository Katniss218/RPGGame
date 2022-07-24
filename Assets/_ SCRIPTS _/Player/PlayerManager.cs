using RPGGame.Assets;
using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Player
{
    /*
    public class SingletonComponent<TFind, T>
        where TFind : Component
        where T : Component 
    {
        private static T instance;

        public T Instance
        {
            get
            {
                if( instance == null )
                {
                    TFind temp = Object.FindObjectOfType<TFind>();
                    if( temp == null )
                    {
                        throw new System.Exception( $"Tried accessing a singleton component {typeof( TFind )}, but none exist." );
                    }
                    instance = temp.GetComponent<T>();
                }

                return instance;
            }
        }

        public static implicit operator T( SingletonComponent<TFind, T> obj )  // doesn't work with expressions and member access.
        {
            return obj.Instance;
        }
    }*/

    public static class PlayerManager
    {
        private static Transform __player = null;
        /// <summary>
        /// The player's root object, null if none exist.
        /// </summary>
        public static Transform Player
        {
            get
            {
                if( __player == null )
                {
                    __player = Object.FindObjectOfType<PlayerMovementController>()?.transform;
                }
                return __player;
            }
        }

        public static GameObject SpawnPlayer()
        {
            GameObject player = Object.Instantiate( AssetManager.Prefabs.Get( "Prefabs/Player" ) );

            HealthHandler health = player.GetComponent<HealthHandler>();
            if( health != null )
            {
                health.onHealthChange?.AddListener( PlayerUI.Instance.healthbarUI.OnHealthOrMaxHealthChange );
                health.onMaxHealthChange?.AddListener( PlayerUI.Instance.healthbarUI.OnHealthOrMaxHealthChange );
            }

            return player;
        }
    }
}