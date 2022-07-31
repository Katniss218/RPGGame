using RPGGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RPGGame.Progression.Dialogues
{
    public static class DialogueManager
    {
        /// <summary>
        /// Stores all dialogues at runtime.
        /// </summary>
        static Dictionary<Guid, Dialogue> allDialogues = new Dictionary<Guid, Dialogue>();

        static DialogueUI Dui;

        public static DialogueOption currentOption;

        public static List<Dialogue> GetDialogues( DialogueSpeaker speaker )
        {
            throw new NotImplementedException();
        }

        public static void StartDialogue( DialogueSpeaker speaker, Dialogue dialogue )
        {
            if( Dui != null )
            {
                Object.Destroy( Dui );
                Dui = null;
            }
            Dui = DialogueUI.Create();

#warning TODO - fix this.
            if( true )// dialogue.StartingOption.InteractorSpeaks )
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
#warning TODO - the dialogue speaker needs to progress too.
            if( currentOption != null && !currentOption.ContraOptions.Contains( option ) )
            {
                throw new Exception( "The option must be one of the ContraOptions of the current dialogue option" );
            }

            currentOption = option;
            Dui.AddSpoken( displayName, option );
            Dui.ClearSelect();
            Dui.AddSelect( option.ContraOptions );
        }
    }
}