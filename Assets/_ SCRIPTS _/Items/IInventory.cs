using RPGGame.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    public interface IInventory : ISerializedComponent
    {
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

        bool IsValidIndex( int slotIndex, Item item );

        /// <summary>
        /// Returns the slot indices for all valid slots.
        /// </summary>
        List<int> GetAllSlots();

        (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack );
        (ItemStack, int orig) GetItemSlot( int slotIndex );
        List<(ItemStack, int orig)> GetItemSlots();

        int? CanSetItem( ItemStack itemStack, int slotIndex, Reason reason = Reason.GENERIC );
        int AddItem( ItemStack itemStack, int slotIndex, Reason reason = Reason.GENERIC );

        int? CanRemoveItem( int slotIndex, Reason reason = Reason.GENERIC );
        int RemoveItem( int amount, int slotIndex, Reason reason = Reason.GENERIC );

        public GameObject gameObject { get; }
        public Transform transform { get; }
    }
}