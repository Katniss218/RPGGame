using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items.Inventories
{
    /// <summary>
    /// Used to distinguish pickups.
    /// </summary>
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class PickupInventory : ListInventory
    {
        [field: NonSerialized]
        public float CreatedTime { get; private set; }

        void OnEnable()
        {
            CreatedTime = Time.time;
        }
    }
}