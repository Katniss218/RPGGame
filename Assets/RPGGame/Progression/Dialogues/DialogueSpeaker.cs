using RPGGame.Interactions;
using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    public abstract class DialogueSpeaker : RPGObjectComponent
    {
        [field: SerializeField]
        public string DisplayName { get; set; }

        public void StartDialogue()
        {
            DialogueManager.StartDialogue( this, DialogueManager.GetDialogues( this ).First() );
        }

        public void StopDialogue()
        {
            DialogueManager.StopDialogue( this, DialogueManager.GetDialogues( this ).First() );
        }

        public abstract void ProgressDialogue( DialogueUI dui, List<DialogueOption> currentOptions );
    }
}