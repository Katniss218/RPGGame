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
        private ItemStack[] Equip = new ItemStack[6];

        public ItemStack EquipHand => Equip[0];
        public ItemStack EquipOffhand => Equip[1];

        public ItemStack EquipHead => Equip[2];
        public ItemStack EquipBody => Equip[3];
        public ItemStack EquipLegs => Equip[4];
        public ItemStack EquipFeet => Equip[5];

        void Awake()
        {
            SetSize( 6, 12 );
            for( int i = 0; i < Equip.Length; i++ )
            {
                Equip[i] = ItemStack.Empty;
            }

            inventorySlots[0, 0] = ItemSlot.BlockingSlot();
            inventorySlots[5, 0] = ItemSlot.BlockingSlot();

            inventorySlots[0, 11] = ItemSlot.BlockingSlot();
            inventorySlots[5, 11] = ItemSlot.BlockingSlot();
        }

#warning TODO - Add a "reason" for the item being picked up and dropped off, to filter slots based on what's happening (do not auto-pick up to equipment slots, but player can manually put weapons there)
        // reasons:
        // - GENERIC
        // - INVENTORY_REARRANGEMENT
        // this should be up to the slot to implement.

        // dropping off: on generic, should throw the item on the ground, rearrangement shouldn't, because we're "throwing" it to the cursor slot thingy instead, so it's not lost.

        void Start()
        {
            SetItem( new ItemStack( AssetManager.GetItem( "item.axe" ), 1 ), -1 );
        }

        public int MapSlotIndexToEquipIndex( int slotIndex )
        {
            // -1 => 0
            // -2 => 1
            // -3 => 2
            // etc.
            return -slotIndex - 1;
        }

        public int MapEquipIndexToSlotIndex( int equipIndex )
        {
            // -1 => 0
            // -2 => 1
            // -3 => 2
            // etc.
            return -(equipIndex + 1);
        }

        public override List<int> GetAllValidIndices()
        {
            List<int> equip = new List<int>() { -6, -5, -4, -3, -2, -1 };
            equip.AddRange( base.GetAllValidIndices() );

            return equip;
        }

        /// <summary>
        /// Returns the complete list of items in the inventory.
        /// </summary>
        public override List<(ItemStack, int orig)> GetItemSlots()
        {
            List<(ItemStack, int orig)> items = new List<(ItemStack, int orig)>();

            for( int i = 0; i < Equip.Length; i++ )
            {
                if( Equip[i].IsEmpty )
                {
                    continue;
                }

                items.Add( (Equip[i], MapEquipIndexToSlotIndex( i )) );
            }
            items.AddRange( base.GetItemSlots() );

            return items;
        }

        public override (ItemStack, int orig) GetItemSlot( int slotIndex )
        {
            if( slotIndex < 0 )
            {
                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                return (Equip[equipIndex], slotIndex);
            }

            return base.GetItemSlot( slotIndex );
        }

        public override int? CanFit( ItemStack itemStack, int slotIndex )
        {
            if( slotIndex < 0 )
            {
                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                if( Equip[equipIndex].CanStackWith( itemStack ) )
                {
                    return Equip[equipIndex].AmountToAdd( itemStack, false );
                }
                return null;
            }

            return base.CanFit( itemStack, slotIndex );
        }

        public override int SetItem( ItemStack itemStack, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            if( slotIndex < 0 )
            {
                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                Equip[equipIndex].Add( itemStack, false );

                Equip[equipIndex] = itemStack.Copy();

                onPickup?.Invoke( new IInventory.PickupEventInfo()
                {
                    Self = this,
                    Reason = reason,
                    Item = itemStack.Item,
                    Amount = itemStack.Amount,
                    SlotOrigin = slotIndex
                } );

                return itemStack.Amount;
            }

            return base.SetItem( itemStack, slotIndex, reason );
        }

        public override int RemoveItem( int amount, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            if( slotIndex < 0 )
            {
                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                Item item = Equip[equipIndex].Item;
                int amountRemoved = Equip[equipIndex].Sub( amount );

                onDrop?.Invoke( new IInventory.DropEventInfo()
                {
                    Self = this,
                    Reason = reason,
                    Item = item,
                    Amount = amountRemoved,
                    SlotOrigin = slotIndex
                } );

                return amountRemoved;
            }

            return base.RemoveItem( amount, slotIndex, reason );
        }
    }
}