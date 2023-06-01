#if UNITY_EDITOR
using RPGGame.Items;
using RPGGame.Progression.Dialogues;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPGGame.Editor
{
    public class DialogueEditorWindow : EditorWindow
    {
        [MenuItem( "Window/RPGGame/Dialogue Editor" )]
        public static void ShowWindow()
        {
            GetWindow( typeof( DialogueEditorWindow ) );
        }

        void OnGUI()
        {
            //UnityEditor.Editor editor = UnityEditor.Editor.CreateEditor( dialogue );

            //editor.OnInspectorGUI();



            //UsableItem d = ScriptableObject.CreateInstance<UsableItem>();

            if( GUILayout.Button( "Add New Item" ) )
            {
                //d.dialogue.Add( new Dialogue() );
            }

            //SerializedObject obj = new SerializedObject( d );
            //SerializedProperty prop = obj.FindProperty( "dialogue" );
           // DrawProperties( prop, true );



            //dialogue = (EditorGUILayout.ObjectField( "Dialogue", dialogue, typeof( Dialogue ), false ) as Dialogue);
        }

        void DrawProperties( SerializedProperty prop, bool drawChildren )
        {
            string lastPropPath = string.Empty;

            foreach( SerializedProperty p in prop )
            {
                if( p.isArray && p.propertyType == SerializedPropertyType.Generic )
                {
                    EditorGUILayout.BeginHorizontal();
                    p.isExpanded = EditorGUILayout.Foldout( p.isExpanded, p.displayName );
                    EditorGUILayout.EndHorizontal();

                    if( p.isExpanded )
                    {
                        EditorGUI.indentLevel++;
                        DrawProperties( p, drawChildren );
                        EditorGUI.indentLevel--;
                    }
                }
                else
                {
                    if( !string.IsNullOrEmpty( lastPropPath ) && p.propertyPath.Contains( lastPropPath ) )
                    {
                        continue;
                    }
                    lastPropPath = p.propertyPath;
                    EditorGUILayout.PropertyField( p, drawChildren );
                }
            }
        }
    }
}
#endif