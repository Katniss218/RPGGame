using RPGGame.Progression.Dialogues;
using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Player
{
    public class PlayerDialogueSpeaker : DialogueSpeaker
    {
        public override void ProgressDialogue( DialogueUI dui, List<DialogueOption> currentOptions )
        {
#warning TODO - locks.
            dui.AddSelect( currentOptions );
        }
    }
}