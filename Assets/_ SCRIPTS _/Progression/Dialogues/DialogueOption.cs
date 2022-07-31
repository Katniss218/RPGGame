using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    [Serializable]
    public class DialogueOption
    {
        // GUID

        [field: SerializeField]
        public string Text { get; set; }

        [field: SerializeField]
        public bool InteractorSpeaks { get; set; }

        /// <summary>
        /// The list of conditions that must be met in order to enable this dialogue option.
        /// </summary>
        [field: SerializeField]
        public List<ProgressionLock> Conditions { get; set; }

        /// <summary>
        /// The dialogue options following this specific option.
        /// </summary>
        [field: SerializeField]
        public List<DialogueOption> ContraOptions { get; set; }

        public bool CanSay()
        {
            foreach( var condition in Conditions )
            {
                if( !condition.IsMet() )
                {
                    return false;
                }
            }

            return true;
        }
    }
}