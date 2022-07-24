using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class PlayerUI : MonoBehaviour
    {
        public HealthbarUI healthbarUI;

        private static PlayerUI __playerUI = null;
        /// <summary>
        /// The player's root object, null if none exist.
        /// </summary>
        public static PlayerUI Instance
        {
            get
            {
                if( __playerUI == null )
                {
                    __playerUI = Object.FindObjectOfType<PlayerUI>();
                }
                return __playerUI;
            }
        }
    }
}