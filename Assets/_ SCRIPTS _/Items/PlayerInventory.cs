using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public sealed class PlayerInventory : GridInventory
    {
        public (Item i, int amt) EquipHand;
        public (Item i, int amt) EquipOffhand;

        public (Item i, int amt) EquipHead;
        public (Item i, int amt) EquipBody;
        public (Item i, int amt) EquipLegs;

        void Awake()
        {
            inventorySlots = new ItemSlot[6, 12];
            MakeEmptyWithNoBlocking();

            inventorySlots[0, 0] = ItemSlot.BlockingSlot();
            inventorySlots[5, 0] = ItemSlot.BlockingSlot();

            inventorySlots[0, 11] = ItemSlot.BlockingSlot();
            inventorySlots[5, 11] = ItemSlot.BlockingSlot();
        }

#warning TODO - finish adding equipment slots. Add a "reason" for the item being picked up and dropped off, to filter slots based on what's happening (do not auto-pick up to equipment slots, but player can manually put weapons there)
        // reasons:
        // - GENERIC
        // - INVENTORY_REARRANGEMENT
        // this should be up to the slot to implement.

        // dropping off: on generic, should throw the item on the ground, rearrangement shouldn't, because we're "throwing" it to the cursor slot thingy instead, so it's not lost.

        void Start()
        {
            PickUp( AssetManager.GetItem( "item.axe" ), 1, -1 );
        }

        public override (Item i, int amt, int orig) GetItemSlot( int slotIndex )
        {
            if( slotIndex == -1 )
            {
                return (EquipHand.i, EquipHand.amt, -1);
            }

            return base.GetItemSlot( slotIndex );
        }

        public override int? CanPickUp( Item item, int slotIndex )
        {
            if( slotIndex == -1 )
            {
                // We know the slot is not full.
                if( EquipHand.i == null )
                {
                    return item.MaxStack;
                }
                if( EquipHand.i.ID == item.ID )
                {
                    return EquipHand.i.MaxStack - EquipHand.amt;
                }
            }

            return base.CanPickUp( item, slotIndex );
        }

        public override int PickUp( Item item, int amount, int slotIndex )
        {
            if( slotIndex == -1 )
            {
                EquipHand = (item, amount);

                onPickup?.Invoke( new IInventory.PickupEventInfo()
                {
                    Amount = 1,
                    Item = item,
                    Self = this,
                    SlotOrigin = -1
                } );

                return amount;
            }

            return base.PickUp( item, amount, slotIndex );
        }

        public override int Drop( int? amount, int slotIndex )
        {
#warning TODO - I don't like how this is duplicated here, and in the grid inventory (and basically in any other inventory that supports rearrangement).
            // Moving this to a separate helper class would probably be the best idea.

            if( slotIndex == -1 )
            {
                if( amount == null )
                {
                    amount = EquipHand.amt;
                }

                int amountToRemove = Mathf.Min( amount.Value, EquipHand.amt );

                if( amountToRemove <= 0 )
                {
                    return 0;
                }

                Item item = EquipHand.i;
                EquipHand.amt -= amountToRemove;
                if( EquipHand.amt == 0 )
                {
                    EquipHand = (null, 0);
                }

                onDrop?.Invoke( new IInventory.DropEventInfo()
                {
                    Amount = 1,
                    Item = item,
                    Self = this,
                    SlotOrigin = -1
                } );

                return amountToRemove;
            }

            return base.Drop( amount, slotIndex );
        }
    }
}