using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
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

            inventorySlots[0, 0].MakeBlockingSlot();
            inventorySlots[5, 0].MakeBlockingSlot();

            inventorySlots[0, 11].MakeBlockingSlot();
            inventorySlots[5, 11].MakeBlockingSlot();
        }

        void Start()
        {
            AddItem( new ItemStack( AssetManager.GetItem( "$asset:item:item.spear" ), 1 ), -1, IInventory.Reason.INVENTORY_REARRANGEMENT );
        }

        public static int MapSlotIndexToEquipIndex( int slotIndex )
        {
            // -1 => 0
            // -2 => 1
            // -3 => 2
            // etc.
            return -slotIndex - 1;
        }

        public static int MapEquipIndexToSlotIndex( int equipIndex )
        {
            return -(equipIndex + 1);
        }

        public override List<int> GetAllSlots()
        {
            List<int> equip = new List<int>() { -6, -5, -4, -3, -2, -1 };
            equip.AddRange( base.GetAllSlots() );

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

        public override int? CanSetItem( ItemStack itemStack, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            if( slotIndex < 0 )
            {
                // Accessing the equipment slot is only possible by dragging an item there.

                if( reason != IInventory.Reason.INVENTORY_REARRANGEMENT )
                {
                    return null;
                }
                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                if( Equip[equipIndex].CanStackWith( itemStack ) )
                {
                    return Equip[equipIndex].AmountToAdd( itemStack, false );
                }
                return null;
            }

            return base.CanSetItem( itemStack, slotIndex );
        }

        public override int AddItem( ItemStack itemStack, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
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

            return base.AddItem( itemStack, slotIndex, reason );
        }

        public override int? CanRemoveItem( int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            if( slotIndex < 0 )
            {
                // Accessing the equipment slot is only possible by dragging an item there.
                if( reason != IInventory.Reason.INVENTORY_REARRANGEMENT )
                {
                    return null;
                }

                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                if( Equip[equipIndex].IsEmpty )
                {
                    return null;
                }

                return Equip[equipIndex].Amount;
            }

            return base.CanRemoveItem( slotIndex, reason );
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