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

                    if( inter.IsInteracting( Main.Player ) )
                    {
                        inter.StopInteracting( Main.Player );
                    }
                    else
                    {
                        inter.StartInteracting( Main.Player );
                    }
                }
            }
        }
    }
}