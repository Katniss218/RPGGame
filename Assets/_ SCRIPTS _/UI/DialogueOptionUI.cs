using RPGGame.Progression.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class DialogueOptionUI : MonoBehaviour
    {
        public DialogueOption Option { get; private set; }
        public DialogueUI DialogueUI { get; set; }

        [SerializeField] TMPro.TextMeshProUGUI text;

        public void Init( DialogueOption option, DialogueUI dui )
        {
            this.DialogueUI = dui;
            this.Option = option;
            this.text.text = option.Text;
        }

        public void OnClickCallback()
        {
            DialogueManager.ProgressDialogue( Option.Speaker, Option );
        }
    }
}