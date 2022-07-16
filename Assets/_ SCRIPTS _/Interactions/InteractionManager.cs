using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGGame.Interactions
{
    public class InteractionManager : MonoBehaviour
    {
        void Update()
        {
            HandlePlayerInteractions();
        }

        void HandlePlayerInteractions()
        {
            if( Input.GetMouseButtonDown( 1 ) )
            {
                if( EventSystem.current.IsPointerOverGameObject() )
                {
                    return;
                }

                Ray ray = Main.Camera.ScreenPointToRay( Input.mousePosition );
                if( Physics.Raycast( ray, out RaycastHit hitInfo, float.PositiveInfinity ) )
                {
                    Interactible inter = hitInfo.collider.GetComponent<Interactible>();

                    if( inter == null )
                    {
                        return;
                    }

                    if( !inter.IsInteracting( null ) )
                    {
#warning TODO - this is quite ugly, as it assumes something is attached to this oninteract and will stop the interaction later.
                        inter.StartInteracting( null );
                    }
                }
            }
        }
    }
}