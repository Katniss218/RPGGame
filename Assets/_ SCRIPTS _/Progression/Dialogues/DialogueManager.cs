using RPGGame.Player;
using RPGGame.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

#warning TODO - this is becoming a surprisingly common pattern (we'll have the same thing with quests, locks, and other, and RPGObjects already kinda have the same thing, they also have empty guid prevention). Maybe add a new class to serve as an encapsulated registry?
        public static Dialogue Get( Guid guid )
        {
            if( allDialogues.TryGetValue( guid, out Dialogue dialogue ) )
            {
                return dialogue;
            }
            return null;
        }
        
        public static Guid Get( Dialogue dialogue )
        {
            return dialogue.Guid;
        }

        public static void Register( Dialogue dialogue )
        {
            if( dialogue.Guid == Guid.Empty )
            {
                dialogue.Guid = Guid.NewGuid();
            }
            allDialogues[dialogue.Guid] = dialogue;
        }

        public static void Clear()
        {
            allDialogues = new Dictionary<Guid, Dialogue>();
        }

        // -=-=-=-=-=-=-=-=-=-

        public static List<Dialogue> GetDialogues( DialogueSpeaker speaker )
        {
            return new List<Dialogue>()
            {
                allDialogues.Values.First()
            };
        }

        // -=-=-=-=-=-=-=-=-=-

        static DialogueUI dui;

        static DialogueOption currentOption;

        public static void StartDialogue( DialogueSpeaker speaker, Dialogue dialogue )
        {
            if( dui != null )
            {
                Object.Destroy( dui );
                dui = null;
            }
            dui = DialogueUI.Create();

            if( dialogue.StartingOption.Speaker == PlayerManager.Player.GetComponent<DialogueSpeaker>() )
            {
                dui.AddSelect( new List<DialogueOption>() { dialogue.StartingOption } );
            }
            else
            {
                ProgressDialogue( speaker.DisplayName, dialogue.StartingOption );
            }
        }

        public static void StopDialogue( DialogueSpeaker speaker, Dialogue dialogue )
        {
            if( dui == null )
            {
                return;
            }
            Object.Destroy( dui.gameObject );
            dui = null;
            currentOption = null;
        }

#warning TODO - Better to split this into AIDialogueSpeaker and PlayerDialogueSpeaker derived from DialogueSpeaker.
        public static void ProgressDialogue( string displayName, DialogueOption option )
        {
#warning TODO - the dialogue speaker needs to progress too.
            if( currentOption != null && !currentOption.FollowingOptions.Contains( option ) )
            {
                throw new Exception( "The option must be one of the ContraOptions of the current dialogue option" );
            }

            currentOption = option;
            dui.AddSpoken( displayName, option );
            dui.ClearSelect();
            dui.AddSelect( option.FollowingOptions );
        }
    }
}