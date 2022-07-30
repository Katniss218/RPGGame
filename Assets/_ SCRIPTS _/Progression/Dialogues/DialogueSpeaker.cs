using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    public class DialogueSpeaker : MonoBehaviour
    {
        [field: SerializeField]
        public string DisplayName { get; set; }

        [field: SerializeField]
        public Dialogue Dialogue { get; set; }

        public void StartDialogue()
        {

        }
    }
}