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

            public Item Item;
            public int Amount;
            public int SlotOrigin;
        }

        public class DropEventInfo
        {
            public IInventory Self;

            public Item Item;
            public int Amount;
            public int SlotOrigin;
        }

        public class ResizeEventInfo
        {
            public IInventory Self;
        }

        UnityEvent<PickupEventInfo> onPickup { get; }
        UnityEvent<DropEventInfo> onDrop { get; }
        UnityEvent<ResizeEventInfo> onResize { get; }

        bool IsEmpty();

        void Clear();

        (List<(int index, int amt)>, int leftover) GetNeededSlots( Item item, int amount );
        (Item i, int amt, int orig) GetItemSlot( int slotIndex );
        List<(Item i, int amt, int orig)> GetItemSlots();

        int? CanPickUp( Item item, int slotIndex );
        int PickUp( Item item, int amount );
        int PickUp( Item item, int amount, int slotIndex );

        int? CanDrop( int slotIndex );
        int Drop( Item item, int amount );
        int Drop( int? amount, int slotIndex );

        public GameObject gameObject { get; }
        public Transform transform { get; }
    }
}