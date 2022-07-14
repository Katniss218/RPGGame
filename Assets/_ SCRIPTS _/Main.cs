using RPGGame.Items;
using RPGGame.Player;
using RPGGame.UI;
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

        private static Main __instance = null;
        public static Main Instance
        {
            get
            {
                if( __instance == null )
                {
                    __instance = Object.FindObjectOfType<Main>();
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
                    __cameraController = Object.FindObjectOfType<CameraController>();
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
                    __player = Object.FindObjectOfType<PlayerMovementController>()?.transform;
                }
                return __player;
            }
        }

#warning TODO - turn this into a prefab.
        [SerializeField] private GameObject ___mobHud;

        private static GameObject __mobHud = null;
        public static GameObject MobHud
        {
            get
            {
                if( __mobHud == null )
                {
                    __mobHud = Instance.___mobHud;
                }
                return __mobHud;
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

        private static PlayerRespawnPoint __playerRespawnPoint = null;
        public static PlayerRespawnPoint PlayerRespawnPoint
        {
            get
            {
                if( __playerRespawnPoint == null )
                {
                    __playerRespawnPoint = Transform.FindObjectOfType<PlayerRespawnPoint>();
                }
                return __playerRespawnPoint;
            }
        }

        private void Start()
        {
            const string GAME_SCENE_NAME = "game";

            AsyncOperation sceneLoadOper = SceneManager.LoadSceneAsync( GAME_SCENE_NAME, LoadSceneMode.Additive );

            sceneLoadOper.completed += ( AsyncOperation oper ) =>
            {
                SceneManager.SetActiveScene( SceneManager.GetSceneByName( GAME_SCENE_NAME ) );
                CameraController.FollowTarget = Player;

                PlayerMovementController pmc = Player.GetComponent<PlayerMovementController>();
                if( pmc == null )
                {
                    return;
                }

                pmc.CameraPivot = CameraController.transform;

                PlayerInventoryUI.CreateUIWindow( Player.GetComponent<PlayerInventory>() );
            };
        }
    }
}