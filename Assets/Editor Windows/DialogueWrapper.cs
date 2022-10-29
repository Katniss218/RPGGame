using RPGGame.Progression.Dialogues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Editor
{
    /// <summary>
    /// This is gonna hold the dialogues serialized in the editor.
    /// Alternatively we could also use the entire game scene, as it is a burner scene and is loaded at runtime later.
    /// </summary>
    public class DialogueWrapper : MonoBehaviour
    {
#warning TODO - all dialogues have empty guid because Unity can't serialize it.
        [field: SerializeField]
        public List<Dialogue> Dialogues { get; set; } = new List<Dialogue>() { new Dialogue() };

        public class EventInfo
        {
            public string Value;
        }

        public UnityEvent<EventInfo> onTest;

        public event Action<EventInfo> onTest2;

        public void Callback( EventInfo e )
        {
            Debug.Log( "test" );
        }
    }
}