using RPGGame.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items.Inventories
{
    /// <summary>
    /// A generic inventory that can hold items.
    /// </summary>
    public interface IInventory : ISerializedComponent
    {
        // The inventory should only be concerned with *storing* items.
        // Any additional functionality should be obtained with additional components.

        public class PickupEventInfo
        {
            public IInventory Self;
            public Reason Reason;

            public Item Item;
            public int Amount;
            public int SlotOrigin;
        }

        public class DropEventInfo
        {
            public IInventory Self;
            public Reason Reason;

            public Item Item;
            public int Amount;
            public int SlotOrigin;
        }

        public class ResizeEventInfo
        {
            public IInventory Self;
        }

        /// <summary>
        /// Contains the possible reasons for picking up / dropping the item.
        /// </summary>
        public enum Reason
        {
            /// <summary>
            /// Any generic picking up, dropping after death, etc.
            /// </summary>
            GENERIC,

            /// <summary>
            /// Clicking around in an inventory.
            /// </summary>
            INVENTORY_REARRANGEMENT,

            /// <summary>
            /// Saving or loading (serialization).
            /// </summary>
            PERSISTENCE
        }

        UnityEvent<PickupEventInfo> onPickup { get; }
        UnityEvent<DropEventInfo> onDrop { get; }
        UnityEvent<ResizeEventInfo> onResize { get; }

        /// <summary>
        /// Checks if the slot index is a valid index for a given item (NOT regarding stacking items, in the case of grid inventories some slots would make the item out of bounds).
        /// </summary>
        /// <returns>True if the slot is a valid slot index, and can accept the item, otherwise false.</returns>
        bool IsValidIndex( int slotIndex, Item item );

        /// <summary>
        /// Returns the slot indices for all valid slots.
        /// </summary>
        IEnumerable<int> GetAllSlots();

        /// <summary>
        /// Returns a sequence of slot and amount pairs that can be used at the time to fill the inventory with the specified items.
        /// A leftover value is provided, if the inventory can't fit all the items.
        /// </summary>
#warning TODO - probably best to add the reason to this too.
        (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack );

        /// <summary>
        /// Returns the item at a given slot.
        /// </summary>
        (ItemStack, int orig) GetItemSlot( int slotIndex );

        /// <summary>
        /// Returns the complete list of items in the inventory.
        /// </summary>
        IEnumerable<(ItemStack, int orig)> GetItemSlots();

        /// <summary>
        /// Checks whether the item can be added to a particular slot, given a reason.
        /// </summary>
        /// <returns>If the item is compatible, the amount of space in the slot (including 0 for a compatible item, but not enough space). Null if the item is incompatible.</returns>
        int? CanAddItem( ItemStack itemStack, int slotIndex, Reason reason = Reason.GENERIC );
        /// <summary>
        /// Adds an amount of specified item into a slot.
        /// </summary>
        /// <remarks>
        /// This should throw an exception if the operation would replace (delete) a different item already present in the inventory.
        /// This shouldn't overstack the items.
        /// </remarks>
        /// <returns>The amount of items that were actually added.</returns>
        int AddItem( ItemStack itemStack, int slotIndex, Reason reason = Reason.GENERIC );

        /// <summary>
        /// Checks whether the item can be removed from a particular slot, given a reason.
        /// </summary>
        /// <returns>If the item is compatible, the amount of space in the slot. If the item is incompatible or the slot is empty, null.</returns>
        int? CanRemoveItem( int slotIndex, Reason reason = Reason.GENERIC );
        /// <summary>
        /// Removes an amount of the item from a slot.
        /// </summary>
        /// <remarks>
        /// This should throw an exception if there is no item to remove.
        /// </remarks>
        /// <returns>The amount of items that were actually removed.</returns>
        int RemoveItem( int amount, int slotIndex, Reason reason = Reason.GENERIC );

        //  -----------------------------

        //      Force a MonoBehaviour

        public GameObject gameObject { get; }
        public Transform transform { get; }

        T GetComponent<T>();
    }
}