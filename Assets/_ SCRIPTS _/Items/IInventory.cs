using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    public interface IInventory
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
            GENERIC,
            INVENTORY_REARRANGEMENT
        }

        UnityEvent<PickupEventInfo> onPickup { get; }
        UnityEvent<DropEventInfo> onDrop { get; }
        UnityEvent<ResizeEventInfo> onResize { get; }

        bool IsEmpty();

        void Clear();

        bool IsValidIndex( int slotIndex, Item item );

        List<int> GetAllValidIndices();

        (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack );
        (ItemStack, int orig) GetItemSlot( int slotIndex );
        List<(ItemStack, int orig)> GetItemSlots();

        int? CanFit( ItemStack itemStack, int slotIndex );
        int TryAdd( ItemStack itemStack, Reason reason = Reason.GENERIC );
        int SetItem( ItemStack itemStack, int slotIndex, Reason reason = Reason.GENERIC );

        int? CanRemove( int slotIndex );
        int TryRemove( ItemStack itemStack, Reason reason = Reason.GENERIC );
        int RemoveItem( int amount, int slotIndex, Reason reason = Reason.GENERIC );

        public GameObject gameObject { get; }
        public Transform transform { get; }
    }
}