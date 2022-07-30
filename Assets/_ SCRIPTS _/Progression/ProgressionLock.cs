using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression
{
    /// <summary>
    /// Represents an abstract type of prerequisite that must be met in order to do something.
    /// </summary>
    public abstract class ProgressionLock : ISerializedComponent
    {
        // So quests, dialogues, killed mobs, etc.


#warning TODo - we need a "scoreboard" of progression criteria.
        /// <summary>
        /// Returns true if the lock is not currently restricting anything.
        /// </summary>
        public virtual bool IsMet()
        {
            return false;
        }

        public abstract JObject GetData();

        public abstract void SetData( JObject data );
    }
}