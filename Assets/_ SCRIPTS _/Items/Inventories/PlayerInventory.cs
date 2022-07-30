using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items.Inventories
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
    public sealed class PlayerInventory : GridInventory
    {
        private ItemStack[] Equip = new ItemStack[6]
        {
            ItemStack.Empty,
            ItemStack.Empty,
            ItemStack.Empty,
            ItemStack.Empty,
            ItemStack.Empty,
            ItemStack.Empty,
        };

        public const int SLOT_HAND = -1;
        public const int SLOT_OFFHAND = -2;
        public const int SLOT_HEAD = -3;
        public const int SLOT_CHEST = -4;
        public const int SLOT_LEGS = -5;
        public const int SLOT_FEET = -6;


        public ItemStack EquipHand => Equip[MapSlotIndexToEquipIndex( SLOT_HAND )];
        public ItemStack EquipOffhand => Equip[MapSlotIndexToEquipIndex( SLOT_OFFHAND )];

        public ItemStack EquipHead => Equip[MapSlotIndexToEquipIndex( SLOT_HEAD )];
        public ItemStack EquipBody => Equip[MapSlotIndexToEquipIndex( SLOT_CHEST )];
        public ItemStack EquipLegs => Equip[MapSlotIndexToEquipIndex( SLOT_LEGS )];
        public ItemStack EquipFeet => Equip[MapSlotIndexToEquipIndex( SLOT_FEET )];

        void Awake()
        {
            SetSize( 6, 12 );
            
            slots[GetSlotIndex( 0, 0, SizeX )].MakeNonExistent();
            slots[GetSlotIndex( 5, 0, SizeX )].MakeNonExistent();

            slots[GetSlotIndex( 0, 11, SizeX )].MakeNonExistent();
            slots[GetSlotIndex( 5, 11, SizeX )].MakeNonExistent();
        }

        void Start()
        {
            AddItem( new ItemStack( AssetManager.Items.Get( "item.spear" ), 1 ), SLOT_HAND, IInventory.ChangeReason.PERSISTENCE );
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

        public override IEnumerable<int> GetAllSlots()
        {
            List<int> equip = new List<int>()
            {
                SLOT_FEET,
                SLOT_LEGS,
                SLOT_CHEST,
                SLOT_HEAD,
                SLOT_OFFHAND,
                SLOT_HAND
            };
            equip.AddRange( base.GetAllSlots() );

            return equip;
        }

        /// <summary>
        /// Returns the complete list of items in the inventory.
        /// </summary>
        public override IEnumerable<(ItemStack, int orig)> GetItemSlots()
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

        public override int? CanAddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( slotIndex < 0 )
            {
                // Do not allow the picked up items, etc to fall into the equipment slots directly.

                if( reason == IInventory.ChangeReason.GENERIC )
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

            return base.CanAddItem( itemStack, slotIndex );
        }

        public override int AddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
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

        public override int? CanRemoveItem( int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( slotIndex < 0 )
            {
                int equipIndex = MapSlotIndexToEquipIndex( slotIndex );

                if( Equip[equipIndex].IsEmpty )
                {
                    return null;
                }

                return Equip[equipIndex].Amount;
            }

            return base.CanRemoveItem( slotIndex, reason );
        }

        public override int RemoveItem( int amount, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
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


        public override JObject GetData()
        {
            return IInventoryEx.GetData( this );
        }

        public override void SetData( JObject data )
        {
            IInventoryEx.SetData( this, data );
        }
    }
}