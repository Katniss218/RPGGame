using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class MobInventory : Inventory
    {
        public Item EquipHand;

        public override void Clear()
        {
            Item tempitem = EquipHand;
            EquipHand = null;

            onDrop?.Invoke( new DropEventInfo()
            {
                Amount = 1,
                Item = tempitem,
                OriginSlot = Vector2Int.zero,
                Self = this
            } );

            base.Clear();
        }
    }
}