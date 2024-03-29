using Newtonsoft.Json.Linq;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items.Inventories
{
    [DisallowMultipleComponent]
    public class GridInventory : MonoBehaviour, IInventory
    {
        /// <summary>
        /// Represents a single cell in the inventory grid.
        /// </summary>
        [Serializable]
        protected class GridSlot : ItemStack
        {
            // The inventory is a grid comprised of ItemSlots.
            // An item can take up multiple slots (diablo/metin2 style).
            // An item in the inventory grid has one slot as its origin (least significant coordinate).
            // - All slots belonging to a specific item instance point to their origin.

            // ItemSlot can be a "blocking" slot, meaning that specific slot can't be used.
            // - This can be used to shape an inventory in fun ways.

            // Other derived classes can provide their own systems.

            public static new GridSlot Empty( int index )
            {
                return new GridSlot( null, 0, index, index );
            }

            public void MakeNonExistent()
            {
                this.MakeEmpty();
                this.IsNonExistent = true;
            }

            /// <summary>
            /// If true, the slot "doesn't exist" (can't accept any item and isn't returned in the list of all slots).
            /// </summary>
            public bool IsNonExistent = false;

            /// <summary>
            /// Points at the top-left corner of each item.
            /// </summary>
            public int OriginIndex;

            /// <summary>
            /// The coordinates of the slot in an inventory.
            /// </summary>
            public int Index;

            public bool IsOrigin => OriginIndex == Index;

            public GridSlot( Item item, int amount, int originIndex, int index ) : base( item, amount )
            {
                this.OriginIndex = originIndex;
                this.Index = index;
            }

            public override void MakeEmpty()
            {
                this.OriginIndex = this.Index;

                base.MakeEmpty();
            }

            /// <summary>
            /// Copies the slot with a change in coordinates.
            /// </summary>
            public GridSlot Copy( int index )
            {
                if( this.IsEmpty )
                {
                    return Empty( index );
                }

                return new GridSlot( this.Item, this.Amount, this.OriginIndex, index );
            }
        }

        [SerializeField]
        protected GridSlot[] slots;

        [field: SerializeField]
        public int SizeX { get; private set; }

        public int SizeY => slots.Length / SizeX; // If the array isn't a "full" grid, it'll skip the last row.

        [SerializeField] UnityEvent<IInventory.PickupEventInfo> __onPickup;
        public UnityEvent<IInventory.PickupEventInfo> onPickup { get => __onPickup; }
        [SerializeField] UnityEvent<IInventory.DropEventInfo> __onDrop;
        public UnityEvent<IInventory.DropEventInfo> onDrop { get => __onDrop; }
        [SerializeField] UnityEvent<IInventory.ResizeEventInfo> __onResize;
        public UnityEvent<IInventory.ResizeEventInfo> onResize { get => __onResize; }


        private bool IsValidIndex( int slotIndex, int sizeX = 0, int sizeY = 0 )
        {
            (int x, int y) = GetSlotCoords( slotIndex, SizeX );

            if( x < 0 || y < 0 || (x + sizeX) > SizeX || (y + sizeY) > SizeY )
            {
                return false;
            }

            return true;
        }

        public static (int x, int y) GetSlotCoords( int slotIndex, int InvSizeX )
        {
            if( slotIndex < 0 )
            {
                throw new Exception( "Slot index can't be less than 0" );
            }
            return (slotIndex % InvSizeX, slotIndex / InvSizeX);
        }

        public static int GetSlotIndex( int x, int y, int InvSizeX )
        {
            if( x < 0 || y < 0 || x >= InvSizeX )
            {
                throw new Exception( "Slot position can't be less than 0 or more than or equal to size" );
            }
            return (y * InvSizeX) + x;
        }

        //
        //
        //

        public void SetSize( int sizeX, int sizeY )
        {
            slots = new GridSlot[sizeX * sizeY];

            for( int i = 0; i < slots.Length; i++ )
            {
                slots[i] = GridSlot.Empty( i );
            }

            onResize?.Invoke( new IInventory.ResizeEventInfo()
            {
                Self = this
            } );
        }

        public virtual bool IsValidIndex( int slotIndex, Item item )
        {
            return IsValidIndex( slotIndex, item.Size.x, item.Size.y );
        }

        public virtual IEnumerable<int> GetAllSlots()
        {
            List<int> indexArray = new List<int>();

            foreach( var slot in slots )
            {
                if( slot.IsNonExistent )
                {
                    continue;
                }
                indexArray.Add( slot.Index );
            }

            return indexArray;
        }

        public virtual (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentNullException( "ItemStack can't be null or empty." );
            }

            // Returns a string of (position, amount) tuples, into which we can then sequentially put the items we want, along with how many items can fit there.
            // Returns the amount of items we can't fit, 0 if all fit.

            // We want to insert into slots left-to-right first, top-to-bottom second.
            // if no amount of the item can't be put into the current slot, we skip to the next.
            // we need to keep track of the used up slots in order to reject the future slots. So the item can't be put into the slot, if it was previously marked as available, and is already used up.

            // we also don't have to check the rows that would put the bottom of the rectangle out of bounds.

            // a new data structure, holding the amount that will be put there in each grid. for non-origin slots, slots that can't have the item put into them, and slots that are already used up by the process.

            int amountLeft = itemStack.Amount;

            List<(int index, int amt)> orderedSlots = new List<(int index, int amt)>();

            int itemSizeX = itemStack.Item.Size.x;
            int itemSizeY = itemStack.Item.Size.y;

            int[,] amounts = new int[SizeX, SizeY];

            int lastRowPlusOne = SizeY - itemSizeY + 1;
            int lastColumnPlusOne = SizeX - itemSizeX + 1;

            for( int y = 0; y < SizeY; y++ )
            {
                for( int x = 0; x < SizeX; x++ )
                {
                    amounts[x, y] = -1;
                }
            }

#warning TODO - this should take the existing partially full slots into account first.

            // inventory starts top-left already. So it aligns with the order of appending items.
            for( int y = 0; y < lastRowPlusOne; y++ )
            {
                for( int x = 0; x < lastColumnPlusOne; x++ )
                {
                    // skip over already processed slots.
                    if( amounts[x, y] != -1 )
                    {
                        continue;
                    }

                    int? spaceLeft = CanAddItem( itemStack, GetSlotIndex( x, y, SizeX ) );

                    // If the item can't fit, mark as 0 and move on.
                    if( spaceLeft == null )
                    {
                        amounts[x, y] = 0;
                        continue;
                    }

                    int amountToPut = Mathf.Min( amountLeft, spaceLeft.Value );
                    amountLeft -= amountToPut;

                    GetNeededSlots_PickUp( ref amounts, amountToPut, x, y, x + itemSizeX, y + itemSizeY );
                    orderedSlots.Add( (GetSlotIndex( x, y, SizeX ), amountToPut) );

                    if( amountLeft < 0 )
                    {
                        throw new Exception( "AmountLeft was negative, wtf!" );
                    }

                    if( amountLeft == 0 )
                    {
                        return (orderedSlots, 0);
                    }
                }
            }

            return (orderedSlots, amountLeft);
        }

        private void GetNeededSlots_PickUp( ref int[,] amounts, int amt, int minX, int minY, int maxX, int maxY )
        {
            // fill the origin with amt, fill the rest with 0s.
            for( int y = minY; y < maxY; y++ )
            {
                for( int x = minX; x < maxX; x++ )
                {
                    if( x == minX && y == minY )
                    {
                        amounts[x, y] = amt;
                        continue;
                    }

                    amounts[x, y] = 0;
                }
            }
        }

        public virtual (ItemStack, int orig) GetItemSlot( int slotIndex )
        {
            GridSlot slot = slots[slotIndex];
            if( slot.IsNonExistent )
            {
                throw new InvalidOperationException( "Can't get an item from a blocking slot" );
            }

            return (slot.Copy(), slot.OriginIndex);
        }

        public virtual IEnumerable<(ItemStack, int orig)> GetItemSlots()
        {
            List<(ItemStack, int orig)> items = new List<(ItemStack, int orig)>();

            foreach( var slot in slots )
            {
                if( slot.IsNonExistent || slot.IsEmpty || !slot.IsOrigin )
                {
                    continue;
                }

                items.Add( (slot, slot.Index) );
            }

            return items;
        }

        //
        //      PICK UP (ADD)
        //

        public virtual int? CanAddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentException( "ItemStack can't be null or empty." );
            }

            if( !IsValidIndex( slotIndex, itemStack.Item ) )
            {
                return null;
            }

            // We must be placing the item over its own origin, or into a grid of all empties.

            (int posX, int posY) = GetSlotCoords( slotIndex, SizeX );

            for( int y = posY; y < posY + itemStack.Item.Size.y; y++ )
            {
                for( int x = posX; x < posX + itemStack.Item.Size.x; x++ )
                {
                    GridSlot slot = slots[GetSlotIndex( x, y, SizeX )];

                    // Blocking slot or different item.
                    if( slot.IsNonExistent || (!slot.IsEmpty && !slot.Item.CanStackWith( itemStack.Item )) )
                    {
                        return null;
                    }

                    // The item is the same, but we're not placing it over the origin.
                    if( !slot.IsEmpty && slot.OriginIndex != slotIndex )
                    {
                        return null;
                    }
                }
            }

            GridSlot clickedSlot = slots[slotIndex];

            return clickedSlot.AmountToAdd( itemStack, false );
        }

        /// <summary>
        /// Picks up an item directly into a slot.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="pos">The slot to pick the item up to (doesn't need to be origin)</param>
        /// <returns>Returns now many items were added to the inventory.</returns>
        public virtual int AddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
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

            int originIndex = slots[slotIndex].OriginIndex;
            GridSlot originSlot = slots[originIndex];

            int amountAdded = originSlot.Add( itemStack, false );

            // Copy to the other slots.
            (int posX, int posY) = GetSlotCoords( slotIndex, SizeX );

            for( int y = posY; y < posY + itemStack.Item.Size.y; y++ )
            {
                for( int x = posX; x < posX + itemStack.Item.Size.x; x++ )
                {
                    slots[GetSlotIndex( x, y, SizeX )] = originSlot.Copy( GetSlotIndex( x, y, SizeX ) );
                }
            }

            onPickup?.Invoke( new IInventory.PickupEventInfo()
            {
                Self = this,
                Reason = reason,
                Item = itemStack.Item,
                Amount = amountAdded,
                SlotOrigin = originIndex
            } );

            return amountAdded;
        }

        //
        //      DROP (REMOVE)
        //

        /// <summary>
        /// Returns the amount of items that can be dropped from this slot. Null if it can't be dropped.
        /// </summary>
        /// <param name="pos">(doesn't need to be origin)</param>
        /// <returns></returns>
        public virtual int? CanRemoveItem( int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            // false if you click outside the area, or on a blocking slot.

            if( !IsValidIndex( slotIndex, 0, 0 ) )
            {
                return null;
            }

            GridSlot slot = slots[slotIndex];

            if( slot.IsNonExistent || slot.IsEmpty )
            {
                return null;
            }

            return slot.Amount;
        }

        /// <summary>
        /// Drops an item from a slot.
        /// </summary>
        /// <param name="amount">How many items to drop. Null for the entire stack.</param>
        /// <param name="pos">The slot you want to drop from (doesn't need to be origin).</param>
        /// <returns>Returns how many items were dropped from the inventory.</returns>
        public virtual int RemoveItem( int amount, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            GridSlot clickedSlot = slots[slotIndex];

            if( clickedSlot.IsNonExistent )
            {
                throw new InvalidOperationException( "Tried dropping from a blocking slot" );
            }

            if( clickedSlot.IsEmpty )
            {
                throw new InvalidOperationException( "Tried dropping from an empty slot" );
            }

            int origin = clickedSlot.OriginIndex;
            GridSlot originSlot = slots[origin];

            Item item = originSlot.Item;

            int amountRemoved = originSlot.Sub( amount );

            // Copy to the other slots.
            (int posX, int posY) = GetSlotCoords( slotIndex, SizeX );

            for( int y = posY; y < posY + item.Size.y; y++ )
            {
                for( int x = posX; x < posX + item.Size.x; x++ )
                {
                    slots[GetSlotIndex( x, y, SizeX )] = originSlot.Copy( GetSlotIndex( x, y, SizeX ) );
                }
            }

            onDrop?.Invoke( new IInventory.DropEventInfo()
            {
                Self = this,
                Reason = reason,
                Item = item,
                Amount = amountRemoved,
                SlotOrigin = origin
            } );

            return amountRemoved;
        }

        //
        //
        //

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( "inv\n" );
            for( int y = 0; y < SizeY; y++ )
            {
                for( int x = 0; x < SizeX; x++ )
                {
                    GridSlot slot = slots[GetSlotIndex( x, y, SizeX )];
                    if( slot.IsNonExistent )
                    {
                        sb.Append( "#" );
                        continue;
                    }
                    sb.Append( slot.IsEmpty ? "-" : "?" );
                }
                sb.Append( '\n' );
            }

            return sb.ToString();
        }

        //  ---------------------

        //      SERIALIZATION
        //

        public virtual JObject GetData()
        {
            JObject data = IInventoryEx.GetData( this );
            data.Add( "Size", new Vector2Int( SizeX, SizeY ).ToJson() );

            return data;
        }

        public virtual void SetData( JObject data )
        {
            Vector2Int size = data["Size"].ToVector2Int();

            this.SetSize( size.x, size.y );
            IInventoryEx.SetData( this, data );
        }
    }
}