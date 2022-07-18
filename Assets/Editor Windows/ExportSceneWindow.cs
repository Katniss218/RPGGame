using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using RPGGame.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace RPGGame.Editor
{
    public class ExportSceneWindow : EditorWindow
    {
        //string myString = "Hello World";
       // bool groupEnabled;
        //bool myBool = true;
        //float myFloat = 1.23f;

        // Add menu item named "My Window" to the Window menu
        [MenuItem( "Window/RPGGame/Scene Export" )]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            GetWindow( typeof( ExportSceneWindow ) );
        }

        void OnGUI()
        {
            if( GUILayout.Button( "Export" ) )
            {
                JObject data = SerializationManager.SaveScene();

                string savedText = JsonConvert.SerializeObject( data, Formatting.Indented );

                System.IO.File.WriteAllText( Main.SAVE_FILE, savedText );

                Debug.Log( $"Exported to '{Main.SAVE_FILE}'." );
            }

            //
            /*
            GUILayout.Label( "Base Settings", EditorStyles.boldLabel );
            myString = EditorGUILayout.TextField( "Text Field", myString );

            groupEnabled = EditorGUILayout.BeginToggleGroup( "Optional Settings", groupEnabled );
            {
                myBool = EditorGUILayout.Toggle( "Toggle", myBool );
                myFloat = EditorGUILayout.Slider( "Slider", myFloat, -3, 3 );
                EditorGUILayout.EndToggleGroup();
            }*/
        }
    }
}