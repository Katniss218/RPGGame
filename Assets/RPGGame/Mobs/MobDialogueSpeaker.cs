using RPGGame.Progression.Dialogues;
using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame.Mobs
{
    public class MobDialogueSpeaker : DialogueSpeaker
    {
        public override void ProgressDialogue( DialogueUI dui, List<DialogueOption> currentOptions )
        {
            DialogueOption chosenOption = currentOptions.First();
#warning TODO - null/empty?
            dui.AddSpoken( chosenOption );
        }
    }
}