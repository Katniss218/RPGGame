using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    [Serializable]
    public class Dialogue
    {
        // GUID

        [field: SerializeField]
        public DialogueOption StartingOption { get; set; }


        // NEW dialogue system dev:

        /*
        
        - A "dialogue" will be a detached structure, not assigned to any object.

        - The dialogue itself defines which NPCs (Dialogue speakers) can speak it.

        - When you interact with something, it's gonna look up all dialogues for that object.

        - Objects are defined via their RPGObject ID (guid).

        - There is a non-serialized "setup" object for global things in the play scene.
            - it will be used to hold a dialoguewrapper MonoBehaviour that will have a list of dialogues on it.

        - The play scene is all loaded at the same time.


        */
    }
}