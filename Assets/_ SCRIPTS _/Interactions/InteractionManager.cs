using RPGGame.Player;
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

                Ray ray = CameraController.Camera.ScreenPointToRay( Input.mousePosition );
                if( Physics.Raycast( ray, out RaycastHit hitInfo, float.PositiveInfinity ) )
                {
                    Interactible inter = hitInfo.collider.GetComponent<Interactible>();

                    if( inter == null )
                    {
                        return;
                    }

                    if( inter.IsInteracting( PlayerManager.Player ) )
                    {
                        inter.StopInteracting( PlayerManager.Player );
                    }
                    else
                    {
                        inter.StartInteracting( PlayerManager.Player );
                    }
                }
            }
        }
    }
}