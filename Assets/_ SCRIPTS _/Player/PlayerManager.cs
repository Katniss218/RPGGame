using RPGGame.Assets;
using RPGGame.Items;
using RPGGame.UI;
using RPGGame.UI.Windows;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Player
{
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

        /// <summary>
        /// Spawns the player object.
        /// </summary>
        public static GameObject SpawnPlayer()
        {
            GameObject player = Object.Instantiate( AssetManager.Prefabs.Get( "Prefabs/player" ) );

            return player;
        }

        /// <summary>
        /// Hooks up the spawned player to the pre-existing UI elements.
        /// </summary>
        public static void SetUpPlayerHooks()
        {
            CameraController.Instance.FollowTarget = Player;

            PlayerMovementController pmc = Player.GetComponent<PlayerMovementController>();
            if( pmc != null )
            {
                pmc.CameraPivot = CameraController.Instance.transform;
                PlayerInventoryUI.CreateUIWindow( Player.GetComponent<PlayerInventory>(), Player );
            }

            HealthHandler health = Player.GetComponent<HealthHandler>();
            if( health != null )
            {
                health.onHealthChange?.AddListener( MainUI.Instance.PlayerHealthbar.OnHealthOrMaxHealthChange );
                health.onMaxHealthChange?.AddListener( MainUI.Instance.PlayerHealthbar.OnHealthOrMaxHealthChange );
            }

            PlayerInventory inv = Player.GetComponent<PlayerInventory>();
            if( inv != null )
            {
                MainUI.Instance.ShowInventoryBtn.onClick?.AddListener( () =>
                {
                    if( UIWindow.ExistsAny<PlayerInventoryUI>() )
                    {
                        return;
                    }
                    PlayerInventoryUI.CreateUIWindow( inv, inv.transform );
                } );
            }
        }
    }
}