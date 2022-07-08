using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGGame
{
    public class Main : MonoBehaviour
    {
        private void Start()
        {
            AsyncOperation sceneLoadOper = SceneManager.LoadSceneAsync( "game", LoadSceneMode.Additive );
        }
    }
}