using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using RPGGame.Items;
using RPGGame.Player;
using RPGGame.Serialization;
using RPGGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace RPGGame
{
    [DisallowMultipleComponent]
    public sealed class Main : MonoBehaviour
    {
        public const string MOB_UI_CANVAS_NAME = "MobHudCanvas";
        public const string GAME_UI_CANVAS_NAME = "GameHudCanvas";
        public const string UI_WINDOW_CANVAS_NAME = "UIWindowCanvas";

        private static Main __instance = null;
        public static Main Instance
        {
            get
            {
                if( __instance == null )
                {
                    __instance = FindObjectOfType<Main>();
                }
                return __instance;
            }
        }

        private static Canvas __mobHudCanvas = null;
        public static Canvas MobHudCanvas
        {
            get
            {
                if( __mobHudCanvas == null )
                {
                    __mobHudCanvas = GameObject.Find( MOB_UI_CANVAS_NAME ).GetComponent<Canvas>();
                }
                return __mobHudCanvas;
            }
        }

        private static Canvas __gameHudCanvas = null;
        public static Canvas GameHudCanvas
        {
            get
            {
                if( __gameHudCanvas == null )
                {
                    __gameHudCanvas = GameObject.Find( GAME_UI_CANVAS_NAME ).GetComponent<Canvas>();
                }
                return __gameHudCanvas;
            }
        }

        private static Canvas __uiWindowCanvas = null;
        public static Canvas UIWindowCanvas
        {
            get
            {
                if( __uiWindowCanvas == null )
                {
                    __uiWindowCanvas = GameObject.Find( UI_WINDOW_CANVAS_NAME ).GetComponent<Canvas>();
                }
                return __uiWindowCanvas;
            }
        }

        private static PlayerRespawnPoint __playerRespawnPoint = null;
        public static PlayerRespawnPoint PlayerRespawnPoint
        {
            get
            {
                if( __playerRespawnPoint == null )
                {
                    __playerRespawnPoint = FindObjectOfType<PlayerRespawnPoint>();
                }
                return __playerRespawnPoint;
            }
        }

        public static string GameDirectory { get => AppDomain.CurrentDomain.BaseDirectory; }

        private void Start()
        {
            SceneSwitcher.AppendScene( SceneSwitcher.MENU_SCENE_NAME, null );
        }

        public static void CreatePickup( Item item, int amount, Vector3 position, Quaternion rotation, bool applyForce )
        {
            const float HEIGHT_OFFSET = 0.25f;
            const float JITTER_RANGE = 0.05f;

            // This helps de-clump the items if multiple are spawned by the same dropping entity.
            Vector3 randomOffset = new Vector3( Random.Range( -JITTER_RANGE, JITTER_RANGE ), HEIGHT_OFFSET, Random.Range( -JITTER_RANGE, JITTER_RANGE ) );

            GameObject go = Instantiate( AssetManager.Prefabs.Get( "Prefabs/pickup" ), position + randomOffset, rotation );
            go.name = "pickup";

            PickupInventory inventory = go.GetComponent<PickupInventory>();
            inventory.SetCapacityAndPickUp( new ItemStack( item, amount ) );

            if( applyForce )
            {
                Rigidbody rigidbody = go.GetComponent<Rigidbody>();

                Vector3 dir = ((rotation * Vector3.forward) + new Vector3( 0.0f, 0.4f, 0.0f )).normalized;
                rigidbody.velocity = dir * 60f;
            }
        }

    }
}