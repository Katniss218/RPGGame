using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame.Items
{
    /// <summary>
    /// Used to distinguish pickups.
    /// </summary>
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
    public class PickupInventory : GridInventory
    {
        public float createdTime;

        private void OnEnable()
        {
            createdTime = Time.time;
        }

        public void SetCapacityAndPickUp( ItemStack itemStack )
        {
            if( itemStack == null )
            {
                throw new ArgumentNullException( "ItemStack can't be null." );
            }
            if( itemStack.IsEmpty )
            {
                throw new ArgumentException( "ItemStack can't be empty." );
            }

            inventorySlots = new ItemSlot[itemStack.Item.Size.x, itemStack.Item.Size.y];

            MakeEmptyWithNoBlocking();

            onResize?.Invoke( new IInventory.ResizeEventInfo()
            {
                Self = this
            } );

            SetItem( itemStack, 0 );
        }

        /// <summary>
        /// Makes the inventory into a perfect grid of empty slots.
        /// </summary>
        void MakeEmptyWithNoBlocking()
        {
            for( int y = 0; y < SizeY; y++ )
            {
                for( int x = 0; x < SizeX; x++ )
                {
                    inventorySlots[x, y] = ItemSlot.Empty( GetSlotIndex( x, y, SizeX ) );
                }
            }
        }
    }
}