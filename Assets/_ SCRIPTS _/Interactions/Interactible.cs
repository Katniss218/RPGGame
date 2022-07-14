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

        public UnityEvent<OnInteractEventInfo> onInteract;

        public void Interact( Transform interactor )
        {
            onInteract?.Invoke( new OnInteractEventInfo()
            {
                Self = this,
                Interactor = interactor
            } );
        }
    }
}