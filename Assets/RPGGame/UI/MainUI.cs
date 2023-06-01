using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    /// <summary>
    /// Holds references to GameCanvas UI elements.
    /// </summary>
    public class MainUI : MonoBehaviour
    {
        [field: SerializeField]
        public HealthbarUI PlayerHealthbar { get; private set; }

        [field: SerializeField]
        public Button ShowInventoryBtn { get; private set; }


        private static MainUI __playerUI = null;
        /// <summary>
        /// The player's root object, null if none exist.
        /// </summary>
        public static MainUI Instance
        {
            get
            {
                if( __playerUI == null )
                {
                    __playerUI = FindObjectOfType<MainUI>();
                }
                return __playerUI;
            }
        }
    }
}