using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame.Items
{
    /// <summary>
    /// Used to distinguish pickups.
    /// </summary>
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class PickupInventory : GridInventory
    {
        public float createdTime;

        private void OnEnable()
        {
            createdTime = Time.time;
        }
    }
}