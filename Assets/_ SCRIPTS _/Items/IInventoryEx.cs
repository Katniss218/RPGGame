using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPGGame.Items
{
    public static class IInventoryEx
    {
        /// <summary>
        /// Picks up a specified amount of specified items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>Returns now many items were added to the inventory.</returns>
        public static int TryAdd( this IInventory inv, ItemStack itemStack, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentNullException( "ItemStack can't be null or empty." );
            }

            (List<(int index, int amt)>, int leftover) seq = inv.GetNeededSlots( itemStack );

            foreach( var slotInfo in seq.Item1 )
            {
                inv.SetItem( itemStack, slotInfo.index, reason );
            }

            return seq.leftover;
        }

        /// <summary>
        /// Drops a specified amount of specified items.
        /// </summary>
        /// <returns>Returns how many items were dropped from the inventory.</returns>
        public static int TryRemove( this IInventory inv, ItemStack itemStack, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentNullException( "ItemStack can't be null or empty." );
            }

            int amountLeft = itemStack.Amount;
            List<(ItemStack, int orig)> items = inv.GetItemSlots();

            foreach( var (existingItemStack, orig) in items )
            {
                int amountDropped = inv.RemoveItem( amountLeft, orig, reason );

                amountLeft -= amountDropped;

                if( amountLeft == 0 )
                {
                    return 0;
                }
            }

            return amountLeft;
        }
    }
}