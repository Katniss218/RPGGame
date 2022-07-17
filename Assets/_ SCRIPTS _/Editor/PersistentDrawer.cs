using UnityEditor;
using UnityEngine;

namespace RPGGame.ObjectCreation
{
    [CustomEditor( typeof( Persistent ) )]
    public class PersistentDrawer : Editor
    {
        private Persistent guidComp;

        public override void OnInspectorGUI()
        {
            if( guidComp == null )
            {
                guidComp = (Persistent)target;
            }

            // Draw label
            EditorGUILayout.LabelField( "Guid", guidComp.GetGuid().ToString() );
        }
    }
}