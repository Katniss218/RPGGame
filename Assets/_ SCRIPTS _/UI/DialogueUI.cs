using RPGGame.Progression.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class DialogueUI : MonoBehaviour
    {
        public DialogueSpeaker DialogueSpeaker { get; set; }

        [SerializeField] TMPro.TextMeshProUGUI mainTextArea;

        public void SetOptions( List<DialogueOption> options )
        {
            // Fills the bottom list with possible replies to choose from.
        }

        public void Speak( DialogueOption option )
        {
            // show the text on the main spoken panel (top)
        }
    }
}