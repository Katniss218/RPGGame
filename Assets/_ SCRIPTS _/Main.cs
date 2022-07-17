using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PersistentObject;
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

        static string REFERENCES_FILE = AppDomain.CurrentDomain.BaseDirectory + "/" + "ref.json";
        static string DATA_FILE = AppDomain.CurrentDomain.BaseDirectory + "/" + "data.json";

        private void Update()
        {
#warning TODO - hook this up to a save / load button.
            if( Input.GetKeyDown( KeyCode.R ) )
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                (JObject references, JObject data) = SerializationManager.GetDataForPersistentObjects();

                string referencesText = JsonConvert.SerializeObject( references, Formatting.Indented );
                string dataText = JsonConvert.SerializeObject( data, Formatting.Indented );

                System.IO.File.WriteAllText( REFERENCES_FILE, referencesText );
                System.IO.File.WriteAllText( DATA_FILE, dataText );

                sw.Stop();
                Debug.LogWarning( "ser: " + sw.ElapsedTicks / 10000f + " ms" );
            }

            if( Input.GetKeyDown( KeyCode.T ) )
            {
                System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                string referencesText = System.IO.File.ReadAllText( REFERENCES_FILE );
                string dataText = System.IO.File.ReadAllText( DATA_FILE );

                JObject references = JsonConvert.DeserializeObject<JObject>( referencesText );
                foreach( var (guid, reference) in references )
                {
                    SpawnPersistentObject( (string)reference["PrefabPath"], (string)reference["Name"], guid, default, default );
                }

                SerializationManager.SetDataForPersistentObjects( JsonConvert.DeserializeObject<JObject>( dataText ) );

                sw.Stop();
                Debug.LogWarning( "deser: " + sw.ElapsedTicks / 10000f + " ms" );
            }
        }

        public static GameObject SpawnPersistentObject( string prefabPath, string name, string guid, Vector3 position, Quaternion rotation )
        {
            GameObject obj = Instantiate( AssetManager.GetPrefab( prefabPath ), position, rotation );
            obj.name = name;

            Persistent persistent = obj.GetComponent<Persistent>();
            if( persistent == null )
            {
                persistent = obj.AddComponent<Persistent>();
            }
            persistent.guid = string.IsNullOrEmpty( guid ) ? Guid.NewGuid().ToString() : guid;
            persistent.PrefabPath = prefabPath;

            return obj;
        }

        public static void CreatePickup( Item item, int amount, Vector3 position, Quaternion rotation, bool applyForce )
        {
            const float HEIGHT_OFFSET = 0.25f;
            const float JITTER_RANGE = 0.05f;

            Vector3 offset = new Vector3( Random.Range( -JITTER_RANGE, JITTER_RANGE ), HEIGHT_OFFSET, Random.Range( -JITTER_RANGE, JITTER_RANGE ) );

            GameObject go = SpawnPersistentObject( "Prefabs/pickup", "pickup", null, position + offset, Quaternion.identity );

            GameObject itemVisual = Instantiate( item.model, go.transform );

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