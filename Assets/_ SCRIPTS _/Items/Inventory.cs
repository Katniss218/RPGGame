using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour
    {
        public class PickupEventInfo
        {
            public Inventory Self;

            public Item Item;
            public int Amount;
            public Vector2Int SlotOrigin;
        }

        public class DropEventInfo
        {
            public Inventory Self;

            public Item Item;
            public int Amount;
            public Vector2Int SlotOrigin;
        }

        public class ResizeEventInfo
        {
            public Inventory Self;
        }

        /// <summary>
        /// Represents a single cell in the inventory grid.
        /// </summary>
        [Serializable]
        public class ItemSlot
        {
            // The inventory is a grid comprised of ItemSlots.
            // An item can take up multiple slots (diablo/metin2 style).
            // An item in the inventory grid has one slot as its origin (least significant coordinate).
            // - All slots belonging to an item point to that slot.

            // ItemSlot can be a "blocking" slot, meaning that specific slot can't be used.
            // - This can be used to shape an inventory in fun ways.

            // Other derived classes can provide their own systems.

            public static bool IsBlockingSlot( ItemSlot slot ) => slot == null;

            public static bool PointsTo( ItemSlot slot, ItemSlot originSlot ) => slot.Origin == originSlot.Coordinates;

            public static ItemSlot Empty( int posX, int posY )
            {
                Vector2Int coords = new Vector2Int( posX, posY );
                return new ItemSlot( null, 0, coords, coords );
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
            public Vector2Int Origin;

            /// <summary>
            /// The coordinates of the slot in an inventory.
            /// </summary>
            public Vector2Int Coordinates;

            public int MaxAmount => Item.MaxStack;

            public int SpaceLeft => Item.MaxStack - Amount;


            public bool IsOrigin => Origin == Coordinates;
            public bool IsEmpty => Item == null;
            public bool IsFull => Item != null && Amount >= Item.MaxStack;

            public ItemSlot( Item item, int amount, Vector2Int origin, Vector2Int coordinates )
            {
                this.Item = item;
                this.Amount = amount;
                this.Origin = origin;
                this.Coordinates = coordinates;
            }

            /// <summary>
            /// Copies the slot with a change in coordinates.
            /// </summary>
            public ItemSlot Copy( int posX, int posY )
            {
                if( this.IsEmpty )
                {
                    return Empty( posX, posY );
                }

                return new ItemSlot( this.Item, this.Amount, this.Origin, new Vector2Int( posX, posY ) );
            }
        }

        [SerializeField] protected ItemSlot[,] inventorySlots = new ItemSlot[2, 2]; // each slot points to an object containing the reference to the and amount.

        public UnityEvent<PickupEventInfo> onPickup;
        public UnityEvent<DropEventInfo> onDrop;
        public UnityEvent<ResizeEventInfo> onResize;

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

        public bool[,] GetBlockingSlotMask()
        {
            bool[,] mask = new bool[InvSizeX, InvSizeY];
            for( int y = 0; y < InvSizeY; y++ )
            {
                for( int x = 0; x < InvSizeX; x++ )
                {
                    mask[x, y] = ItemSlot.IsBlockingSlot( inventorySlots[x, y] );
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
                    Vector2Int orig = slot.Origin;

                    inventorySlots[x, y] = ItemSlot.Empty( x, y );

                    if( playEvent )
                    {
                        onDrop?.Invoke( new DropEventInfo()
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
                    inventorySlots[x, y] = ItemSlot.Empty( x, y );
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

            onResize?.Invoke( new ResizeEventInfo()
            {
                Self = this
            } );

            PickUp( item, amount, new Vector2Int( 0, 0 ) );
        }

        private bool IsWithinBounds( int minX, int minY )
        {
            if( minX < 0 || minY < 0 || minX > InvSizeX || minY > InvSizeY )
            {
                return false;
            }
            return true;
        }

        private bool IsWithinBounds( int minX, int minY, int maxX, int maxY )
        {
            if( minX > maxX || minY > maxY )
            {
                throw new ArgumentException( "Max must be greater or equal to Min." );
            }
            if( minX < 0 || minY < 0 || maxX > InvSizeX || maxY > InvSizeY )
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns a sequence of slot and amount pairs that can be used at the time to fill the inventory with the specified items.
        /// A leftover value is provided, if the inventory can't fit all the items.
        /// </summary>
        public virtual (List<(Vector2Int pos, int amt)>, int leftover) GetNeededSlots( Item item, int amount )
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

            List<(Vector2Int pos, int amt)> orderedSlots = new List<(Vector2Int pos, int amt)>();

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

                    int? spaceLeft = CanPickUp( item, new Vector2Int( x, y ) );

                    // If the item can't fit, mark as 0 and move on.
                    if( spaceLeft == null )
                    {
                        amounts[x, y] = 0;
                        continue;
                    }

                    int amountToPut = Mathf.Min( amountLeft, spaceLeft.Value );
                    amountLeft -= amountToPut;

                    PickUp( ref amounts, amountToPut, x, y, x + sizeX, y + sizeY );
                    orderedSlots.Add( (new Vector2Int( x, y ), amountToPut) );

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

        public virtual (Item i, int amt, Vector2Int orig) GetItemSlot( Vector2Int pos )
        {
            ItemSlot slot = inventorySlots[pos.x, pos.y];
            if( !slot.IsOrigin )
            {
                slot = inventorySlots[slot.Origin.x, slot.Origin.y];
                pos = slot.Origin;
            }

            if( slot.IsEmpty )
            {
                return (null, 0, pos);
            }

            return (slot.Item, slot.Amount, pos );
        }

        /// <summary>
        /// Returns the complete list of items in the inventory.
        /// </summary>
        public virtual List<(Item i, int amt, Vector2Int orig)> GetItemSlots()
        {
            List<(Item i, int amt, Vector2Int orig)> items = new List<(Item i, int amt, Vector2Int orig)>();

            foreach( var slot in inventorySlots ) // this won't loop over blocking slots cuz they're null, yey!
            {
                if( slot.IsEmpty )
                {
                    continue;
                }
                if( slot.IsOrigin )
                {
                    items.Add( (slot.Item, slot.Amount, slot.Origin) );
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

        public virtual int? CanPickUp( Item item, Vector2Int pos )
        {
            // returns how many items would fit into that slot.
            // returns null for slots that are incompatible or full.
            // returns the maxstack if the slot is empty

            if( !IsWithinBounds( pos.x, pos.y, pos.x + item.Size.x, pos.y + item.Size.y ) )
            {
                return null;
            }

            if( ItemSlot.IsBlockingSlot( inventorySlots[pos.x, pos.y] ) )
            {
                return null;
            }

            Vector2Int origin = inventorySlots[pos.x, pos.y].Origin;
            ItemSlot originSlot = inventorySlots[origin.x, origin.y];

            for( int y = pos.y; y < pos.y + item.Size.y; y++ )
            {
                for( int x = pos.x; x < pos.x + item.Size.x; x++ )
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

            (List<(Vector2Int pos, int amt)>, int leftover) seq = GetNeededSlots( item, amount );

            foreach( var slotInfo in seq.Item1 )
            {
                PickUp( item, slotInfo.amt, slotInfo.pos );
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
        public virtual int PickUp( Item item, int amount, Vector2Int pos )
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

            Vector2Int origin = inventorySlots[pos.x, pos.y].Origin;
            ItemSlot originSlot = inventorySlots[origin.x, origin.y];

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
            originSlot.Origin = origin;

            // Copy to the other slots.
            for( int y = pos.y; y < pos.y + item.Size.y; y++ )
            {
                for( int x = pos.x; x < pos.x + item.Size.x; x++ )
                {
                    inventorySlots[x, y] = originSlot.Copy( x, y );
                }
            }

            onPickup?.Invoke( new PickupEventInfo()
            {
                Self = this,
                Item = item,
                Amount = amountToAdd,
                SlotOrigin = origin
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
        public virtual int? CanDrop( Vector2Int pos )
        {
            // false if you click outside the area, or on a blocking slot.

            if( !IsWithinBounds( pos.x, pos.y ) )
            {
                return null;
            }

            ItemSlot slot = inventorySlots[pos.x, pos.y];

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
            List<(Item i, int amt, Vector2Int orig)> items = GetItemSlots();

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
        public virtual int Drop( int? amount, Vector2Int pos )
        {
            if( amount != null && amount <= 0 )
            {
                throw new ArgumentException( "Amount can't be less than 1." );
            }

            // if the slot is empty, or blocking, it has undefined behaviour.

            ItemSlot slot = inventorySlots[pos.x, pos.y];

            if( ItemSlot.IsBlockingSlot( slot ) )
            {
                throw new InvalidOperationException( "Tried dropping from a blocking slot" );
            }
            if( slot.IsEmpty )
            {
                throw new InvalidOperationException( "Tried dropping from an empty slot" );
            }

            Vector2Int origin = slot.Origin;
            ItemSlot originSlot = inventorySlots[origin.x, origin.y];

            if( amount == null )
            {
                amount = originSlot.Amount;
            }
            int amountToRemove = Mathf.Min( amount.Value, originSlot.Amount );

            originSlot.Amount -= amountToRemove;
            if( originSlot.Amount == 0 )
            {
                originSlot = ItemSlot.Empty( origin.x, origin.y );
            }


            Item item = slot.Item;

            // Copy to the other slots.
            for( int y = pos.y; y < pos.y + item.Size.y; y++ )
            {
                for( int x = pos.x; x < pos.x + item.Size.x; x++ )
                {
                    inventorySlots[x, y] = originSlot.Copy( x, y );
                }
            }

            onDrop?.Invoke( new DropEventInfo()
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