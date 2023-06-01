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
    /// An inventory that contains items in form a 1-dimensional list/array. Any slot can hold an item of any size.
    /// </summary>
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
    public class ListInventory : MonoBehaviour, IInventory
    {
        [SerializeField]
        private ItemStack[] items = new ItemStack[] { };

        public int Size { get => items.Length; }

        [field: SerializeField]
        public UnityEvent<IInventory.PickupEventInfo> onPickup { get; private set; }

        [field: SerializeField]
        public UnityEvent<IInventory.DropEventInfo> onDrop { get; private set; }

        [field: SerializeField]
        public UnityEvent<IInventory.ResizeEventInfo> onResize { get; private set; }

        public void SetSize( int size )
        {
            items = new ItemStack[size];

            for( int i = 0; i < size; i++ )
            {
                items[i] = ItemStack.Empty;
            }

            onResize?.Invoke( new IInventory.ResizeEventInfo()
            {
                Self = this
            } );
        }

        public bool IsValidIndex( int slotIndex, Item item )
        {
            return IsValidIndex( slotIndex );
        }

        private bool IsValidIndex( int slotIndex )
        {
            return slotIndex >= 0 && slotIndex < this.Size;
        }

        public IEnumerable<int> GetAllSlots()
        {
            List<int> slots = new List<int>();
            for( int i = 0; i < this.Size; i++ )
            {
                slots.Add( i );
            }
            return slots;
        }

        public (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            int amountLeft = itemStack.Amount;

            List<(int index, int amt)> orderedSlots = new List<(int index, int amt)>();

            for( int i = 0; i < Size; i++ )
            {
                int? spaceLeft = CanAddItem( itemStack, i );

                if( spaceLeft == null )
                {
                    continue;
                }

                int amountToPut = Mathf.Min( amountLeft, spaceLeft.Value );
                amountLeft -= amountToPut;

                orderedSlots.Add( (i, amountToPut) );
            }

            return (orderedSlots, amountLeft);
        }

        public (ItemStack, int orig) GetItemSlot( int slotIndex )
        {
            return (items[slotIndex].Copy(), slotIndex);
        }

        public IEnumerable<(ItemStack, int orig)> GetItemSlots()
        {
            return items
                .Where( i => !i.IsEmpty )
                .Select( ( item, i ) => (item.Copy(), i) )
                .ToList();
        }

        public int? CanAddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentException( "ItemStack can't be null or empty." );
            }

            if( !IsValidIndex( slotIndex, itemStack.Item ) )
            {
                return null;
            }

            ItemStack clickedSlot = items[slotIndex];

            return clickedSlot.AmountToAdd( itemStack, false );
        }

        public int AddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentException( "ItemStack can't be null or empty." );
            }

            if( !IsValidIndex( slotIndex, itemStack.Item ) )
            {
                throw new ArgumentException( $"Slot index '{slotIndex}' is invalid." );
            }

            if( CanAddItem( itemStack, slotIndex ) == null )
            {
                throw new InvalidOperationException( $"Placing an item in the slot '{slotIndex}' would replace another item." );
            }

            ItemStack originSlot = items[slotIndex];

            int amountAdded = originSlot.Add( itemStack, false );

            onPickup?.Invoke( new IInventory.PickupEventInfo()
            {
                Self = this,
                Reason = reason,
                Item = itemStack.Item,
                Amount = amountAdded,
                SlotOrigin = slotIndex
            } );

            return amountAdded;
        }

        public int? CanRemoveItem( int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( !IsValidIndex( slotIndex ) )
            {
                return null;
            }

            ItemStack slot = items[slotIndex];

            if( slot.IsEmpty )
            {
                return null;
            }

            return slot.Amount;
        }

        public int RemoveItem( int amount, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            ItemStack clickedSlot = items[slotIndex];

            if( clickedSlot.IsEmpty )
            {
                throw new InvalidOperationException( "Tried dropping from an empty slot" );
            }

            Item item = clickedSlot.Item;

            int amountRemoved = clickedSlot.Sub( amount );

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

        //  ---------------------

        //      SERIALIZATION
        //

        public virtual JObject GetData()
        {
            JObject data = IInventoryEx.GetData( this );
            data.Add( "Size", this.Size );

            return data;
        }

        public virtual void SetData( JObject data )
        {
            this.SetSize( (int)data["Size"] );
            IInventoryEx.SetData( this, data );
        }
    }
}