using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Items;
using RPGGame.Player;
using RPGGame.SaveStates;
using RPGGame.Serialization;
using RPGGame.UI;
using RPGGame.UI.Windows;
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

        /// <summary>
        /// Unloads the active scene and loads a new one.
        /// </summary>
        /// <param name="sceneName">The new scene to load.</param>
        /// <param name="onUnload">The action that'll run after the scene is unloaded, and before the new one begins loading.</param>
        /// <param name="onLoad">The action that'll run after the new scene is loaded.</param>
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

        /// <summary>
        /// Additively loads a new scene.
        /// </summary>
        /// <param name="sceneName">The new scene to load.</param>
        /// <param name="onLoad">The action that'll run after the new scene is loaded.</param>
        public static void AppendScene( string sceneName, Action onLoad )
        {
            AsyncOperation sceneLoadOper = SceneManager.LoadSceneAsync( sceneName, LoadSceneMode.Additive );

            sceneLoadOper.completed += ( AsyncOperation oper ) =>
            {
                SceneManager.SetActiveScene( SceneManager.GetSceneByName( sceneName ) );

                onLoad?.Invoke();
            };
        }

        /// <summary>
        /// This is hooked up to the start game button in the main menu.
        /// </summary>
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
            SaveStateManager.Load( null );
        }
    }
}