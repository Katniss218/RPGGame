using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    public interface IReferenceable
    {
#warning TODO - Reference class can take any Referenceable type, extend the registry to work with them.
        // Alternatively:
        // - remove this from here and add 2 dictionaries, for O(1) both way access. Arguably, the Guid -> Object is more important, but we don't have it sped up.

        Guid Guid { get; }
    }

    [Serializable]
    public class Dialogue : IReferenceable
    {
        [field: NonSerialized]
        public Guid Guid { get; set; }

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


        //  ---------------------

        //      SERIALIZATION
        //

        public static implicit operator JToken( Dialogue self )
        {
            return new JObject()
            {
                { "Guid", self.Guid },
                { "StartingOption", self.StartingOption }
            };
        }

        public static explicit operator Dialogue( JToken json )
        {
            return new Dialogue()
            {
                Guid = (Guid)json["Guid"],
                StartingOption = (DialogueOption)json["StartingOption"]
            };
        }
    }
}