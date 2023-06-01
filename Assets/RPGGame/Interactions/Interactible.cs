using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Interactions
{
    public class Interactible : MonoBehaviour
    {
        public class OnInteractEventInfo
        {
            public Interactible Self;
            public Transform Interactor;
        }

        private List<Transform> currentInteractors = new List<Transform>();

        public UnityEvent<OnInteractEventInfo> onStartInteraction;
        public UnityEvent<OnInteractEventInfo> onStopInteraction;

        public bool IsInteracting( Transform interactor )
        {
            return currentInteractors.Contains( interactor );
        }

        public void Interact( Transform interactor )
        {
            if( IsInteracting( interactor ) )
            {
                throw new System.Exception( $"The interactor {interactor} is already interacting with this object." );
            }

            StartInteracting( interactor );
            StopInteracting( interactor );
        }

        public void StartInteracting( Transform interactor )
        {
            if( IsInteracting( interactor ) )
            {
                throw new System.Exception( $"The interactor {interactor} is already interacting with this object." );
            }

            currentInteractors.Add( interactor );
            onStartInteraction?.Invoke( new OnInteractEventInfo()
            {
                Self = this,
                Interactor = interactor
            } );
        }

        public void StopInteracting( Transform interactor )
        {
            if( !IsInteracting( interactor ) )
            {
                throw new System.Exception( $"The interactor {interactor} is not interacting with this object." );
            }

            currentInteractors.Remove( interactor );
            onStopInteraction?.Invoke( new OnInteractEventInfo()
            {
                Self = this,
                Interactor = interactor
            } );
        }
    }
}