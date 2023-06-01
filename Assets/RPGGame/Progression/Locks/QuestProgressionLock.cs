using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Locks
{
    public class QuestProgressionLock : ProgressionLock
    {
        public string Quest { get; set; }
        // Quest not started
        // Quest in progress
        // Quest completed
        // Quest failed

        // TODO - make separate one for quest tasks.

        public override JObject GetData()
        {
            return new JObject()
            {
                { "Quest", Quest }
            };
        }

        public override void SetData( JObject data )
        {

        }
    }
}