using UnityEngine;
using UnityEditor;
using RPGGame.SaveStates;

namespace RPGGame.Editor
{
    public class ExportSceneWindow : EditorWindow
    {
        [SerializeField] string areaId = "default_area";
        [SerializeField] GameObject defaultPlayer;

        [MenuItem( "Window/RPGGame/Scene Export" )]
        public static void ShowWindow()
        {
            GetWindow( typeof( ExportSceneWindow ) );
        }

        void OnGUI()
        {
            areaId = EditorGUILayout.TextField( "Area ID", areaId );
            defaultPlayer = EditorGUILayout.ObjectField( "Default Player", defaultPlayer, typeof( GameObject ), false ) as GameObject;

            if( GUILayout.Button( "Export" ) )
            {
                SaveGameUtils.SaveArea( null, areaId );
                SaveGameUtils.SavePlayer( null, defaultPlayer );
            }
        }
    }
}