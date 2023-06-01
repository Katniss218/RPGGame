using Newtonsoft.Json.Linq;
using RPGGame.Progression.Locks;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Dialogues
{
    [Serializable]
    public class DialogueOption
    {
        [field: SerializeField]
        public string Text { get; set; } = null;

        [field: SerializeField]
        public DialogueSpeaker Speaker { get; set; } = null;

        /// <summary>
        /// The list of conditions that must be met in order to enable this dialogue option.
        /// </summary>
        [field: SerializeField]
        public List<ProgressionLock> Conditions { get; set; } = new List<ProgressionLock>();

        /// <summary>
        /// The dialogue options following this specific option.
        /// </summary>
        [field: SerializeField]
        public List<DialogueOption> FollowingOptions { get; set; } = new List<DialogueOption>();

        public bool CanSay()
        {
            foreach( var condition in Conditions )
            {
                if( !condition.IsMet() )
                {
                    return false;
                }
            }

            return true;
        }

        //  ---------------------

        //      SERIALIZATION
        //

        public static implicit operator JToken( DialogueOption self )
        {
            List<JToken> followingOptions = new List<JToken>();
            foreach( var opt in self.FollowingOptions )
            {
                followingOptions.Add( opt );
            }

            return new JObject()
            {
                { "Text", self.Text },
                { "Speaker", self.Speaker.RpgObject.ObjectRef() },
#warning TODO - serialize locks properly.
               // { "Conditions", ... },
                { "FollowingOptions", new JArray()
                {
                    followingOptions
                } }
            };
        }

        public static explicit operator DialogueOption( JToken json )
        {
            DialogueOption opt = new DialogueOption();
            opt.Text = (string)json["Text"];

            opt.Speaker = Reference.ObjectRef( json["Speaker"] ).GetComponent<DialogueSpeaker>();

            opt.Conditions = new List<ProgressionLock>();

            opt.FollowingOptions = new List<DialogueOption>();
            foreach( JObject opt2Json in json["FollowingOptions"] )
            {
                opt.FollowingOptions.Add( (DialogueOption)opt2Json );
            }

            return opt;
        }
    }
}