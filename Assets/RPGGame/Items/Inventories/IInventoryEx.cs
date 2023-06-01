using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPGGame.Items.Inventories
{
    public static class IInventoryEx
    {
        /// <summary>
        /// Checks whether or not the inventory doesn't contain any items.
        /// </summary>
        /// <returns>True if no items are in the inventory, otherwise false.</returns>
        public static bool IsEmpty( this IInventory inv )
        {
            foreach( var slotIndex in inv.GetAllSlots() )
            {
                (ItemStack slot, _) = inv.GetItemSlot( slotIndex );
                if( !slot.IsEmpty )
                {
                    return false;
                }
            }

            return true;
        }


        /// <summary>
        /// Removes all items from the inventory.
        /// </summary>
        public static void Clear( this IInventory inv, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            foreach( var (itemSlot, orig) in inv.GetItemSlots() )
            {
                Item item = itemSlot.Item;
                int amt = itemSlot.Amount;

                itemSlot.MakeEmpty();

                inv.onDrop?.Invoke( new IInventory.DropEventInfo()
                {
                    Self = inv,
                    Reason = reason,
                    Item = item,
                    Amount = amt,
                    SlotOrigin = orig
                } );
            }
        }


        /// <summary>
        /// Picks up a specified amount of specified items.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        /// <returns>Returns now many items were added to the inventory.</returns>
        public static int TryAdd( this IInventory inv, ItemStack itemStack, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentNullException( "ItemStack can't be null or empty." );
            }

            (List<(int index, int amt)>, int leftover) seq = inv.GetNeededSlots( itemStack );

            foreach( var slotInfo in seq.Item1 )
            {
                inv.AddItem( itemStack, slotInfo.index, reason );
            }

            return seq.leftover;
        }

        /// <summary>
        /// Drops a specified amount of specified items.
        /// </summary>
        /// <returns>Returns how many items were dropped from the inventory.</returns>
        public static int TryRemove( this IInventory inv, ItemStack itemStack, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            if( itemStack == null || itemStack.IsEmpty )
            {
                throw new ArgumentNullException( "ItemStack can't be null or empty." );
            }

            int amountLeft = itemStack.Amount;

            foreach( var (existingItemStack, orig) in inv.GetItemSlots() )
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

        //  ---------------------

        //      SERIALIZATION
        //

        public static JObject GetData( IInventory inv )
        {
            JObject itemsKeyedBySlot = new JObject();
            
            foreach( var (itemStack, slot) in inv.GetItemSlots() )
            {
                itemsKeyedBySlot.Add( slot.ToString(), itemStack );
            }

            return new JObject()
            {
                { "Items", itemsKeyedBySlot }
            };
        }

        public static void SetData( IInventory inv, JObject data )
        {
            inv.Clear( IInventory.ChangeReason.PERSISTENCE );

            foreach( var (slot, itemData) in (JObject)data["Items"] )
            {
                ItemStack item = (ItemStack)itemData;

                inv.AddItem( item, int.Parse( slot ), IInventory.ChangeReason.PERSISTENCE );
            }
        }
    }
}