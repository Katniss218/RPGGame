using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    public static class DialogueManager
    {
        static DialogueUI Dui;

       public static DialogueOption currentOption;

        public static void StartDialogue( DialogueSpeaker speaker, Dialogue dialogue )
        {
            if( Dui != null )
            {
                Object.Destroy( Dui );
                Dui = null;
            }
            Dui = DialogueUI.Create();

            if( dialogue.StartingOption.InteractorSpeaks )
            {
                Dui.AddSelect( new List<DialogueOption>() { dialogue.StartingOption } );
            }
            else
            {
                ProgressDialogue( speaker.DisplayName, dialogue.StartingOption );
            }
        }

        public static void StopDialogue( DialogueSpeaker speaker, Dialogue dialogue )
        {
            if( Dui == null )
            {
                return;
            }
            Object.Destroy( Dui );
            Dui = null;
        }

        public static void ProgressDialogue( string displayName, DialogueOption option )
        {
            if( currentOption != null && !currentOption.ContraOptions.Contains( option ) )
            {
                throw new System.Exception( "The option must be one of the ContraOptions of the current dialogue option" );
            }

            currentOption = option;
            Dui.AddSpoken( displayName, option );
            Dui.ClearSelect();
            Dui.AddSelect( option.ContraOptions );
        }
    }
}