using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    public class Inventory : MonoBehaviour
    {
        public class PickupEventInfo
        {
            public Inventory Self;

            public Item Item;
            public int Slot;
        }

        public class DropEventInfo
        {
            public Inventory Self;

            public Item Item;
            public int Slot;
        }



        [SerializeField] protected Item[] Contents;

        /// <summary>
        /// Return whether or not this inventory doesn't contain ANY items anywhere. Used for e.g. removing pickups, if they're empty.
        /// </summary>
        public virtual bool IsEmpty() => !this.Contents.Any( i => i != null );

        public int Count => Contents.Count( i => i != null );
        public int MaxCapacity => Contents.Length;

        public virtual void SetMaxCapacity( int capacity )
        {
            if( capacity < 0 )
            {
                throw new ArgumentException( "Capacity must be nonnegative." );
            }
            if( !IsEmpty() )
            {
                throw new InvalidOperationException( "Can't set max capacity of an inventory with items in it." );
            }

            this.Contents = new Item[capacity];
        }

        public UnityEvent<PickupEventInfo> onPickup;
        public UnityEvent<DropEventInfo> onDrop;

        public virtual bool CanPickUp( Item item, int? slot = null )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "The parameter 'item' can't be null." );
            }

            if( slot != null )
            {
                return Contents[slot.Value] == null;
            }

            // If the inventory doesn't have enough space, we can't add anything.
            if( Contents.Count( i => i != null ) >= MaxCapacity )
            {
                return false;
            }

            return true;
        }

        public virtual bool CanDrop( int slot )
        {
            return this.Contents[slot] != null;
        }

        /// <summary>
        /// Picks up an item
        /// </summary>
        public virtual void PickUp( Item item, int? slot = null )
        {
            if( item == null )
            {
                throw new ArgumentNullException( "The parameter 'item' can't be null." );
            }

            if( !CanPickUp( item, slot ) )
            {
                throw new InvalidOperationException( $"Can't pick up item '{item}', slot = {slot}." );
            }

            if( slot == null )
            {
                slot = Array.IndexOf( Contents, null );
            }

            Contents[slot.Value] = item;
            onPickup?.Invoke( new PickupEventInfo()
            {
                Self = this,
                Item = item,
                Slot = slot.Value
            } );
        }

        /// <summary>
        /// Drops an item. The item reference must be contained within the inventory.
        /// </summary>
        public virtual void Drop( int slot )
        {
            if( !CanDrop( slot ) )
            {
                throw new InvalidOperationException( $"Can't drop item from slot {slot}." );
            }

            Item item = Contents[slot];
            Contents[slot] = null;
            onDrop?.Invoke( new DropEventInfo()
            {
                Self = this,
                Item = item,
                Slot = slot
            } );
        }

        /// <summary>
        /// Tries to move all the items from one inventory to another inventory.
        /// </summary>
        /// <returns>Null if there was no items to move, otherwise the number of items moved (can be 0).</returns>
        public virtual int? PickUp( Inventory source )
        {
            if( !source.Contents.Any( i => i != null ) )
            {
                return null;
            }

            int movedItems = 0;

            for( int i = 0; i < source.Contents.Length; i++ )
            {
                Item item = source.Contents[i];

                if( !source.CanDrop( i ) )
                {
                    continue;
                }
                if( !CanPickUp( item ) )
                {
                    continue;
                }

                source.Drop( i );
                this.PickUp( item );

                movedItems++;
            }

            return movedItems;
        }
    }
}