using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
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
                    __playerUI = Object.FindObjectOfType<MainUI>();
                }
                return __playerUI;
            }
        }
    }
}