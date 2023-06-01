using Newtonsoft.Json.Linq;
using RPGGame.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Progression.Tracking
{
    /// <summary>
    /// The "memory" of this object. Stores information about the quests, dialogues, and other things.
    /// </summary>
    public class ProgressionTracker : MonoBehaviour, ISerializedComponent
    {
        public TrackDialogue TrackDialogue { get; private set; } = new TrackDialogue();


        public JObject GetData()
        {
            return new JObject();
        }

        public void SetData( JObject data )
        {

        }
    }
}