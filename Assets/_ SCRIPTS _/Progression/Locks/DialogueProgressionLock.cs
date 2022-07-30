using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Locks
{
    public class DialogueProgressionLock : ProgressionLock
    {
        // TODO - Proximity progression lock (used to enable a 3rd NPC walking up to you and barging in).

        public string Dialogue { get; set; }

        public override JObject GetData()
        {
            return new JObject()
            {
                { "Dialogue", Dialogue }
            };
        }

        public override void SetData( JObject data )
        {

        }
    }
}