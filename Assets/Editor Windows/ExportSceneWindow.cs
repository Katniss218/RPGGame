#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using RPGGame.SaveStates;

namespace RPGGame.Editor
{
    public class ExportSceneWindow : EditorWindow
    {
        [SerializeField] string save = SaveGameUtils.DEFAULT_SAVE_STATE;
        [SerializeField] string startingAreaId = "default_area";

        [SerializeField] string areaId = "default_area";
        [SerializeField] GameObject defaultPlayer;

        [MenuItem( "Window/RPGGame/Scene Export" )]
        public static void ShowWindow()
        {
            GetWindow( typeof( ExportSceneWindow ) );
        }

        void OnGUI()
        {
            startingAreaId = EditorGUILayout.TextField( "Starting Area ID", startingAreaId );
            defaultPlayer = EditorGUILayout.ObjectField( "Starting Player", defaultPlayer, typeof( GameObject ), false ) as GameObject;

            if( GUILayout.Button( "Export Save Data" ) )
            {
                SaveGameUtils.SaveSaveData( save, new SaveData()
                {
                    CurrentArea = startingAreaId
                } );
                SaveGameUtils.SavePlayer( save, defaultPlayer );
            }

            Rect rect = EditorGUILayout.GetControlRect( false, 1 );
            rect.height = 1;
            EditorGUI.DrawRect( rect, new Color( 0.5f, 0.5f, 0.5f, 1 ) );

            areaId = EditorGUILayout.TextField( "Area ID", areaId );
            if( GUILayout.Button( "Export Scene as Area" ) )
            {
                SaveGameUtils.SaveArea( save, areaId );
            }
        }
    }
}
#endif