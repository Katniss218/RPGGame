#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using RPGGame.SaveStates;

namespace RPGGame.Editor
{
    public class ExportWindow : EditorWindow
    {
        [SerializeField] string save = SaveGameUtils.DEFAULT_SAVE_STATE;
        [SerializeField] string startingAreaId = "default_area";

        //[SerializeField] string areaId = "default_area";
        [SerializeField] RPGObject defaultPlayer;

        [MenuItem( "Window/RPGGame/Export" )]
        public static void ShowWindow()
        {
            GetWindow( typeof( ExportWindow ) );
        }

        void OnGUI()
        {
            startingAreaId = EditorGUILayout.TextField( "Starting Area ID", startingAreaId );
            defaultPlayer = EditorGUILayout.ObjectField( "Starting Player", defaultPlayer, typeof( RPGObject ), false ) as RPGObject;

            startingAreaId = EditorGUILayout.TextField( "Area ID", startingAreaId );
            if( GUILayout.Button( "Export Save Data" ) )
            {
                SaveStateManager.Save( save, startingAreaId, defaultPlayer );
            }
        }
    }
}
#endif