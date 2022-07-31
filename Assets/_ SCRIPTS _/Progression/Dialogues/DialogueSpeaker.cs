using RPGGame.Interactions;
using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    public class DialogueSpeaker : MonoBehaviour
    {
        [field: SerializeField]
        public string DisplayName { get; set; }
        /*
        [field: SerializeField]
        public List<Dialogue> AvailableDialogues { get; set; }

        public void StartDialogue()
        {
            DialogueManager.StartDialogue( this, AvailableDialogues[0] );

           // DialogueManager.ProgressDialogue( "Player (Hardcoded)", DialogueManager.currentOption.ContraOptions[0] );
        }

        public void StopDialogue()
        {
            DialogueManager.StopDialogue( this, AvailableDialogues[0] );
        }*/
    }
}