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
    public class SceneSwitcher : MonoBehaviour
    {
        public string SceneName;

        public const string GAME_SCENE_NAME = "gameplay_empty";
        public const string MENU_SCENE_NAME = "main_menu";

        public void ChangeScene()
        {
            ChangeScene( SceneName,
                onUnload: () =>
            {
                ItemDragAndDrop.cursorItem.MakeEmpty();
            },
                onLoad: () =>
            {
                // nothing for now.
            } );
        }

        public static void ChangeScene( string sceneName, Action onUnload, Action onLoad )
        {
            AsyncOperation sceneUnloadOper = SceneManager.UnloadSceneAsync( SceneManager.GetActiveScene() );

            sceneUnloadOper.completed += ( AsyncOperation oper ) =>
            {
                onUnload?.Invoke();

                AsyncOperation sceneLoadOper = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Additive );

                sceneLoadOper.completed += ( AsyncOperation oper ) =>
                {
                    SceneManager.SetActiveScene( SceneManager.GetSceneByName( sceneName ) );

                    onLoad?.Invoke();
                };
            };
        }

        public static void AppendScene( string sceneName, Action onLoad )
        {
            AsyncOperation sceneLoadOper = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Additive );

            sceneLoadOper.completed += ( AsyncOperation oper ) =>
            {
                SceneManager.SetActiveScene( SceneManager.GetSceneByName( sceneName ) );

                onLoad?.Invoke();
            };
        }

        public void LoadGame()
        {
            StartGame();
        }

        public static void StartGame()
        {
            ChangeScene( GAME_SCENE_NAME, null, OnPlaySceneLoaded );
        }

        private static void OnPlaySceneLoaded()
        {
            SaveAreaManager.Load( null );

            //----------

            Main.CameraController.FollowTarget = PlayerManager.Player;

            PlayerMovementController pmc = PlayerManager.Player.GetComponent<PlayerMovementController>();
            if( pmc == null )
            {
                return;
            }

            pmc.CameraPivot = Main.CameraController.transform;
            PlayerInventoryUI.CreateUIWindow( PlayerManager.Player.GetComponent<PlayerInventory>(), PlayerManager.Player );
        }
    }
}