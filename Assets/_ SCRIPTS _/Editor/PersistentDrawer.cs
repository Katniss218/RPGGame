using UnityEditor;
using UnityEngine;

namespace RPGGame.ObjectCreation
{
    [CustomEditor( typeof( Persistent ) )]
    public class PersistentDrawer : Editor
    {
        private Persistent component;

        public override void OnInspectorGUI()
        {
            if( component == null )
            {
                component = (Persistent)target;
            }

            // Draw label
            EditorGUILayout.LabelField( "Guid", component.GetGuid().ToString() );
        }
    }
}