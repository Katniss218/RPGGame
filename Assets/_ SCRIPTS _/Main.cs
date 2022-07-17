using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Items;
using RPGGame.Player;
using RPGGame.Serialization;
using RPGGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private static CameraController __cameraController = null;
        public static CameraController CameraController
        {
            get
            {
                if( __cameraController == null )
                {
                    __cameraController = FindObjectOfType<CameraController>();
                }
                return __cameraController;
            }
        }

        private static Camera __camera = null;
        public static Camera Camera
        {
            get
            {
                if( __camera == null )
                {
                    __camera = CameraController.Camera;
                }
                return __camera;
            }
        }

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
                    __player = FindObjectOfType<PlayerMovementController>()?.transform;
                }
                return __player;
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

        private void Start()
        {
            SceneSwitcher.AppendScene( SceneSwitcher.MENU_SCENE_NAME, null );
        }

        static string serializedFile = "save.json";

        private void Update()
        {
#warning TODO - hook this up to a save / load button and a file.
            if( Input.GetKeyDown( KeyCode.R ) )
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                string ser = JsonConvert.SerializeObject( SerializationManager.GetDataForPersistentObjects() );

                sw.Stop();
                Debug.LogWarning( "ser: " + sw.ElapsedTicks / 10000f + " ms" );

                System.IO.File.WriteAllText( AppDomain.CurrentDomain.BaseDirectory + "/" + serializedFile, ser );
            }

            if( Input.GetKeyDown( KeyCode.T ) )
            {
                string ser = System.IO.File.ReadAllText( AppDomain.CurrentDomain.BaseDirectory + "/" + serializedFile );

                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                SerializationManager.SetDataForPersistentObjects( JsonConvert.DeserializeObject<JObject>( ser ) );

                sw.Stop();
                Debug.LogWarning( "deser: " + sw.ElapsedTicks / 10000f + " ms" );
            }
        }
    }
}