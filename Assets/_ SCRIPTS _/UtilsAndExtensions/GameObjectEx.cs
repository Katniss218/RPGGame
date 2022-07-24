using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine
{
    public static class GameObjectEx
    {
        public static RectTransform CreateUI( string name, Transform parent )
        {
            GameObject mainUI = new GameObject( name );
            mainUI.transform.SetParent( parent );

            RectTransform rt = mainUI.AddComponent<RectTransform>();

            return rt;
        }

        public static Bounds GetBounds( this GameObject gameObject )
        {
            Bounds bounds = GetOwnBoundsOnly( gameObject );
            if( bounds.extents.x == 0 )
            {
                bounds = new Bounds( gameObject.transform.position, Vector3.zero );
                foreach( Transform child in gameObject.transform )
                {
                    MeshRenderer childRender = child.GetComponent<MeshRenderer>();
                    if( childRender )
                    {
                        bounds.Encapsulate( childRender.bounds );
                    }
                    else
                    {
                        bounds.Encapsulate( GetBounds( child.gameObject ) );
                    }
                }
            }
            return bounds;
        }

        static Bounds GetOwnBoundsOnly( GameObject gameObject )
        {
            MeshRenderer render = gameObject.GetComponent<MeshRenderer>();
            if( render != null )
            {
                return render.bounds;
            }

            return new Bounds( Vector3.zero, Vector3.zero ); ;
        }
    }
}