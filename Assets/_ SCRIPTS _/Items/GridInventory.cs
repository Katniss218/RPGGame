using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    [DisallowMultipleComponent]
    public class GridInventory : MonoBehaviour, IInventory
    {
        /// <summary>
        /// Represents a single cell in the inventory grid.
        /// </summary>
        [Serializable]
        protected class ItemSlot
        {
            // The inventory is a grid comprised of ItemSlots.
            // An item can take up multiple slots (diablo/metin2 style).
            // An item in the inventory grid has one slot as its origin (least significant coordinate).
            // - All slots belonging to an item point to that slot.

            // ItemSlot can be a "blocking" slot, meaning that specific slot can't be used.
            // - This can be used to shape an inventory in fun ways.

            // Other derived classes can provide their own systems.

            public static bool IsBlockingSlot( ItemSlot slot ) => slot == null;

            public static bool PointsTo( ItemSlot slot, ItemSlot originSlot ) => slot.OriginIndex == originSlot.Index;

            public static ItemSlot Empty( int index )
            {
                return new ItemSlot( null, 0, index, index );
            }

            public static ItemSlot BlockingSlot() => null;

            /// <summary>
            /// The item contained in this slot.
            /// </summary>
            public Item Item;

            /// <summary>
            /// How many items are in this slot?
            /// </summary>
            public int Amount;

            /// <summary>
            /// Points at the top-left corner of each item.
            /// </summary>
            public int OriginIndex;

            /// <summary>
            /// The coordinates of the slot in an inventory.
            /// </summary>
            public int Index;

            public int MaxAmount => Item.MaxStack;

            public int SpaceLeft => Item.MaxStack - Amount;


            public bool IsOrigin => OriginIndex == Index;
            public bool IsEmpty => Item == null;
            public bool IsFull => Item != null && Amount >= Item.MaxStack;

            public ItemSlot( Item item, int amount, int originIndex, int index )
            {
                this.Item = item;
                this.Amount = amount;
                this.OriginIndex = originIndex;
                this.Index = index;
            }

            /// <summary>
            /// Copies the slot with a change in coordinates.
            /// </summary>
            public ItemSlot Copy( int index )
            {
                if( this.IsEmpty )
                {
                    return Empty( index );
                }

                return new ItemSlot( this.Item, this.Amount, this.OriginIndex, index );
            }
        }

        [SerializeField] protected ItemSlot[,] inventorySlots = new ItemSlot[2, 2]; // each slot points to an object containing the reference to the and amount.

        [SerializeField] UnityEvent<IInventory.PickupEventInfo> __onPickup;
        public UnityEvent<IInventory.PickupEventInfo> onPickup { get => __onPickup; }
        [SerializeField] UnityEvent<IInventory.DropEventInfo> __onDrop;
        public UnityEvent<IInventory.DropEventInfo> onDrop { get => __onDrop; }
        [SerializeField] UnityEvent<IInventory.ResizeEventInfo> __onResize;
        public UnityEvent<IInventory.ResizeEventInfo> onResize { get => __onResize; }

        public int InvSizeX => inventorySlots.GetLength( 0 );
        public int InvSizeY => inventorySlots.GetLength( 1 );

        // pos is top-left-based.

        /// <summary>
        /// Checks whether or not the inventory doesn't contain any items.
        /// </summary>
        /// <returns>True if no items are in the inventory, otherwise false.</returns>
        public virtual bool IsEmpty()
        {
            foreach( var slot in inventorySlots ) // this won't loop over blocking slots cuz they're null, yey!
            {
                if( !slot.IsEmpty )
                {
                    return false;
                }
            }

            return true;
        }

        public bool[] GetBlockingSlotMask()
        {
            bool[] mask = new bool[InvSizeX * InvSizeY];
            for( int y = 0; y < InvSizeY; y++ )
            {
                for( int x = 0; x < InvSizeX; x++ )
                {
                    int index = GetSlotIndex( x, y, InvSizeX );
                    mask[index] = ItemSlot.IsBlockingSlot( inventorySlots[x, y] );
                }
            }
            return mask;
        }

        /// <summary>
        /// Removes all items from the inventory.
        /// </summary>
        public virtual void Clear()
        {
            for( int y = 0; y < InvSizeY; y++ )
            {
                for( int x = 0; x < InvSizeX; x++ )
                {
                    ItemSlot slot = inventorySlots[x, y];

                    if( ItemSlot.IsBlockingSlot( slot ) )
                    {
                        continue;
                    }

                    bool playEvent = slot.IsOrigin && !slot.IsEmpty;

                    Item item = slot.Item;
                    int amt = slot.Amount;
                    int orig = slot.OriginIndex;

                    inventorySlots[x, y] = ItemSlot.Empty( GetSlotIndex( x, y, InvSizeX ) );

                    if( playEvent )
                    {
                        onDrop?.Invoke( new IInventory.DropEventInfo()
                        {
                            Self = this,
                            Item = item,
                            Amount = amt,
                            SlotOrigin = orig
                        } );
                    }
                }
            }
        }

        /// <summary>
        /// Makes the inventory into a perfect grid of empty slots.
        /// </summary>
        protected void MakeEmptyWithNoBlocking()
        {
            for( int y = 0; y < InvSizeY; y++ )
            {
                for( int x = 0; x < InvSizeX; x++ )
                {
                    inventorySlots[x, y] = ItemSlot.Empty( GetSlotIndex( x, y, InvSizeX ) );
                }
            }
        }

        public void SetCapacityAndPickUp( Item item, int amount )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "Item can't be null" );
            }
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            inventorySlots = new ItemSlot[item.Size.x, item.Size.y];

            MakeEmptyWithNoBlocking();

            onResize?.Invoke( new IInventory.ResizeEventInfo()
            {
                Self = this
            } );

            PickUp( item, amount, 0 );
        }

        private bool IsValidIndex( int slotIndex, int sizeX = 0, int sizeY = 0 )
        {
            (int x, int y) = GetSlotCoords( slotIndex, InvSizeX );

            if( x < 0 || y < 0 || (x + sizeX) > InvSizeX || (y + sizeY) > InvSizeY )
            {
                return false;
            }

            return true;
        }

        private ItemSlot GetSlot( int slotIndex )
        {
            return inventorySlots[(slotIndex % InvSizeX), (slotIndex / InvSizeX)];
        }

        public static (int x, int y) GetSlotCoords( int slotIndex, int InvSizeX )
        {
            return (slotIndex % InvSizeX, slotIndex / InvSizeX);
        }

        public static int GetSlotIndex( int x, int y, int InvSizeX )
        {
            return (y * InvSizeX) + x;
        }

        /// <summary>
        /// Returns a sequence of slot and amount pairs that can be used at the time to fill the inventory with the specified items.
        /// A leftover value is provided, if the inventory can't fit all the items.
        /// </summary>
        public virtual (List<(int index, int amt)>, int leftover) GetNeededSlots( Item item, int amount )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "Item can't be null" );
            }
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            // Returns a string of (position, amount) tuples, into which we can then sequentially put the items we want, along with how many items can fit there.
            // Returns the amount of items we can't fit, 0 if all fit.

            // We want to insert into slots left-to-right first, top-to-bottom second.
            // if no amount of the item can't be put into the current slot, we skip to the next.
            // we need to keep track of the used up slots in order to reject the future slots. So the item can't be put into the slot, if it was previously marked as available, and is already used up.

            // we also don't have to check the rows that would put the bottom of the rectangle out of bounds.

            // a new data structure, holding the amount that will be put there in each grid. for non-origin slots, slots that can't have the item put into them, and slots that are already used up by the process.

            int amountLeft = amount;

            List<(int index, int amt)> orderedSlots = new List<(int index, int amt)>();

            int invSizeX = InvSizeX;
            int invSizeY = InvSizeY;

            int sizeX = item.Size.x;
            int sizeY = item.Size.y;

            int[,] amounts = new int[invSizeX, invSizeY];

            int lastRowPlusOne = invSizeY - sizeY + 1;
            int lastColumnPlusOne = invSizeX - sizeX + 1;

            for( int y = 0; y < invSizeY; y++ )
            {
                for( int x = 0; x < invSizeX; x++ )
                {
                    amounts[x, y] = -1;
                }
            }

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

                    int? spaceLeft = CanPickUp( item, GetSlotIndex( x, y, InvSizeX ) );

                    // If the item can't fit, mark as 0 and move on.
                    if( spaceLeft == null )
                    {
                        amounts[x, y] = 0;
                        continue;
                    }

                    int amountToPut = Mathf.Min( amountLeft, spaceLeft.Value );
                    amountLeft -= amountToPut;

                    PickUp( ref amounts, amountToPut, x, y, x + sizeX, y + sizeY );
                    orderedSlots.Add( (GetSlotIndex( x, y, InvSizeX ), amountToPut) );

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

        public virtual (Item i, int amt, int orig) GetItemSlot( int index )
        {
            ItemSlot slot = GetSlot( index );
            if( !slot.IsOrigin )
            {
                slot = GetSlot( slot.OriginIndex );
                index = slot.OriginIndex;
            }

            if( slot.IsEmpty )
            {
                return (null, 0, index);
            }

            return (slot.Item, slot.Amount, index);
        }

        /// <summary>
        /// Returns the complete list of items in the inventory.
        /// </summary>
        public virtual List<(Item i, int amt, int orig)> GetItemSlots()
        {
            List<(Item i, int amt, int orig)> items = new List<(Item i, int amt, int orig)>();

            foreach( var slot in inventorySlots ) // this won't loop over blocking slots cuz they're null, yey!
            {
                if( slot.IsEmpty )
                {
                    continue;
                }
                if( slot.IsOrigin )
                {
                    items.Add( (slot.Item, slot.Amount, slot.Index) );
                }
            }

            return items;
        }

        private void PickUp( ref int[,] amounts, int amt, int minX, int minY, int maxX, int maxY )
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

        //
        //      PICK UP (ADD)
        //

        public virtual int? CanPickUp( Item item, int index )
        {
            // returns how many items would fit into that slot.
            // returns null for slots that are incompatible or full.
            // returns the maxstack if the slot is empty

            if( !IsValidIndex( index, item.Size.x, item.Size.y ) )
            {
                return null;
            }

            ItemSlot clickedSlot = GetSlot( index );

            if( ItemSlot.IsBlockingSlot( clickedSlot ) )
            {
                return null;
            }

            int origin = clickedSlot.OriginIndex;
            ItemSlot originSlot = GetSlot( origin );

            (int posX, int posY) = GetSlotCoords( index, InvSizeX );

            for( int y = posY; y < posY + item.Size.y; y++ )
            {
                for( int x = posX; x < posX + item.Size.x; x++ )
                {
                    ItemSlot slot = inventorySlots[x, y]; // The first slot will get compared to itself, no big deal, it's always the same, and if a (1,1) sized item comes in, it never goes across boundaries.

                    if( ItemSlot.IsBlockingSlot( slot ) )
                    {
                        return null;
                    }

                    // If the slot is empty, it's obviously available. We don't need to check it.
                    if( slot.IsEmpty )
                    {
                        continue;
                    }

                    // If any of the slots are full, we are either trying to place across item boundaries or into a full slot.
                    if( slot.IsFull )
                    {
                        return null;
                    }

                    // if any full slot doesn't point to the same item as the first slot, we are trying to place across boundaries.
                    if( !ItemSlot.PointsTo( slot, originSlot ) )
                    {
                        return null;
                    }
                }
            }

            // We know the slot is not full.
            if( originSlot.IsEmpty )
            {
                return item.MaxStack;
            }
            if( originSlot.Item.ID == item.ID )
            {
                return originSlot.SpaceLeft;
            }
            return null;
        }

        /// <summary>
        /// Picks up a specified amount of specified items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>Returns now many items were added to the inventory.</returns>
        public virtual int PickUp( Item item, int amount )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "Item can't be null" );
            }
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            (List<(int index, int amt)>, int leftover) seq = GetNeededSlots( item, amount );

            foreach( var slotInfo in seq.Item1 )
            {
                PickUp( item, slotInfo.amt, slotInfo.index );
            }

            return seq.leftover;
        }

        /// <summary>
        /// Picks up an item directly into a slot.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <param name="pos">The slot to pick the item up to (doesn't need to be origin)</param>
        /// <returns>Returns now many items were added to the inventory.</returns>
        public virtual int PickUp( Item item, int amount, int slotIndex )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "Item can't be null" );
            }
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            // forces every slot in the rect to hold the item.
            // amount is added to the amount already present.

            // if called with values out of bounds, it will throw. Make sure to call 'CanPickUp' first.
            // if the box is across item boundaries it will cover up parts of the items, possibly the origin, rendering it broken.


            int originIndex = GetSlot( slotIndex ).OriginIndex;
            ItemSlot originSlot = GetSlot( originIndex );

            if( !originSlot.IsEmpty && originSlot.Item.ID != item.ID )
            {
                throw new InvalidOperationException( $"Different item already present: {originSlot.Item.ID} => {item.ID}" );
            }

            int amountToAdd;
            if( originSlot.IsEmpty )
            {
                originSlot.Item = item;
                amountToAdd = Mathf.Min( item.MaxStack, amount );
            }
            else
            {
                amountToAdd = Mathf.Min( originSlot.SpaceLeft, amount );
            }

            // Set the amount.
            originSlot.Amount += amountToAdd;
            originSlot.OriginIndex = originIndex;

            // Copy to the other slots.
            (int posX, int posY) = GetSlotCoords( slotIndex, InvSizeX );

            for( int y = posY; y < posY + item.Size.y; y++ )
            {
                for( int x = posX; x < posX + item.Size.x; x++ )
                {
                    inventorySlots[x, y] = originSlot.Copy( GetSlotIndex( x, y, InvSizeX ) );
                }
            }

            onPickup?.Invoke( new IInventory.PickupEventInfo()
            {
                Self = this,
                Item = item,
                Amount = amountToAdd,
                SlotOrigin = originIndex
            } );

            return amountToAdd;
        }

        //
        //      DROP (REMOVE)
        //

        /// <summary>
        /// Returns the amount of items that can be dropped from this slot. Null if it can't be dropped.
        /// </summary>
        /// <param name="pos">(doesn't need to be origin)</param>
        /// <returns></returns>
        public virtual int? CanDrop( int index )
        {
            // false if you click outside the area, or on a blocking slot.

            if( !IsValidIndex( index, 0, 0 ) )
            {
                return null;
            }

            ItemSlot slot = GetSlot( index );

            if( ItemSlot.IsBlockingSlot( slot ) )
            {
                return null;
            }

            if( slot.IsEmpty )
            {
                return null;
            }

            return slot.Amount;
        }

        /// <summary>
        /// Drops a specified amount of specified items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>Returns how many items were dropped from the inventory.</returns>
        public virtual int Drop( Item item, int amount )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "Item can't be null" );
            }
            if( amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            int amountLeft = amount;
            List<(Item i, int amt, int orig)> items = GetItemSlots();

            foreach( var itemStack in items )
            {
                int amountToDrop = Mathf.Min( amountLeft, itemStack.amt );
                Drop( amountToDrop, itemStack.orig );

                amountLeft -= amountToDrop;

                if( amountLeft < 0 )
                {
                    throw new Exception( "AmountLeft was negative, wtf!" );
                }

                if( amountLeft == 0 )
                {
                    return 0;
                }
            }

            return amountLeft;
        }

        /// <summary>
        /// Drops an item from a slot.
        /// </summary>
        /// <param name="amount">How many items to drop. Null for the entire stack.</param>
        /// <param name="pos">The slot you want to drop from (doesn't need to be origin).</param>
        /// <returns>Returns how many items were dropped from the inventory.</returns>
        public virtual int Drop( int? amount, int index )
        {
            if( amount != null && amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            // if the slot is empty, or blocking, it has undefined behaviour.

            ItemSlot clickedSlot = GetSlot( index );

            if( ItemSlot.IsBlockingSlot( clickedSlot ) )
            {
                throw new InvalidOperationException( "Tried dropping from a blocking slot" );
            }
            if( clickedSlot.IsEmpty )
            {
                throw new InvalidOperationException( "Tried dropping from an empty slot" );
            }

            int origin = clickedSlot.OriginIndex;
            ItemSlot originSlot = GetSlot( origin );

            if( amount == null )
            {
                amount = originSlot.Amount;
            }
            int amountToRemove = Mathf.Min( amount.Value, originSlot.Amount );

            originSlot.Amount -= amountToRemove;
            if( originSlot.Amount == 0 )
            {
                originSlot = ItemSlot.Empty( origin );
            }

            Item item = clickedSlot.Item;

            // Copy to the other slots.
            (int posX, int posY) = GetSlotCoords( index, InvSizeX );

            for( int y = posY; y < posY + item.Size.y; y++ )
            {
                for( int x = posX; x < posX + item.Size.x; x++ )
                {
                    inventorySlots[x, y] = originSlot.Copy( GetSlotIndex( x, y, InvSizeX ) );
                }
            }

            onDrop?.Invoke( new IInventory.DropEventInfo()
            {
                Self = this,
                Item = item,
                Amount = amountToRemove,
                SlotOrigin = origin
            } );

            return amountToRemove;
        }

        //
        //
        //

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append( "inv\n" );
            for( int y = 0; y < InvSizeY; y++ )
            {
                for( int x = 0; x < InvSizeX; x++ )
                {
                    ItemSlot slot = inventorySlots[x, y];
                    if( ItemSlot.IsBlockingSlot( slot ) )
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

    }
}