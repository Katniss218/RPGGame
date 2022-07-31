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
    }
}