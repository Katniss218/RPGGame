using Newtonsoft.Json.Linq;
using RPGGame.Progression.Dialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Tracking
{
    /// <summary>
    /// Stores information about which dialogue options a specific object has heard/spoken.
    /// </summary>
    public sealed class TrackDialogue
    {
        public enum State
        {
            /// <summary>
            /// This object has NOT spoken/heard this dialogue option.
            /// </summary>
            NotKnown,
            /// <summary>
            /// This object has spoken/heard this dialogue option.
            /// </summary>
            Known
        }

        private const State DEFAULT_STATE = State.NotKnown;

        Dictionary<DialogueOption, State> tracked = new Dictionary<DialogueOption, State>();

        /// <summary>
        /// Returns the state of this dialogue option.
        /// </summary>
        public State CheckState( DialogueOption dialogueOption )
        {
            if( tracked.TryGetValue( dialogueOption, out State actualState ) )
            {
                return actualState;
            }

            return DEFAULT_STATE;
        }

        public void SetState( DialogueOption dialogueOption, State state )
        {
            tracked[dialogueOption] = state;
        }

        public JObject GetData()
        {
            // serialize each dialogue option with its guid.

            throw new System.NotImplementedException();
        }

        public void SetData( JObject data )
        {
            throw new System.NotImplementedException();
        }
    }
}