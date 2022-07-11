using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class MobInventory : GridInventory
    {
        public Item EquipHand;

        public Item[] Drops;

        public override void Clear()
        {
            Item tempitem = EquipHand;
            EquipHand = null;

            onDrop?.Invoke( new IInventory.DropEventInfo()
            {
                Amount = 1,
                Item = tempitem,
                SlotOrigin = 0,
                Self = this
            } );

            Item[] dropsTemp = Drops;

            Drops = null;

            foreach( var item in dropsTemp )
            {
                onDrop?.Invoke( new IInventory.DropEventInfo()
                {
                    Amount = 1,
                    Item = item,
                    SlotOrigin = 0,
                    Self = this
                } );
            }

            base.Clear();
        }
    }
}