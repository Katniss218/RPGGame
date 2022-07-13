using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class MobInventory : MonoBehaviour, IInventory
    {
        public Item EquipHand;

        public Item[] Drops;

        [SerializeField] UnityEvent<IInventory.PickupEventInfo> __onPickup;
        public UnityEvent<IInventory.PickupEventInfo> onPickup { get => __onPickup; }
        [SerializeField] UnityEvent<IInventory.DropEventInfo> __onDrop;
        public UnityEvent<IInventory.DropEventInfo> onDrop { get => __onDrop; }
        [SerializeField] UnityEvent<IInventory.ResizeEventInfo> __onResize;
        public UnityEvent<IInventory.ResizeEventInfo> onResize { get => __onResize; }

        public bool IsEmpty()
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            Item tempitem = EquipHand;
            EquipHand = null;

            onDrop?.Invoke( new IInventory.DropEventInfo()
            {
                Amount = 1,
                Item = tempitem,
                SlotOrigin = 0,
                Self = this
            } );

            Item[] dropsTemp = Drops;

            Drops = null;

            foreach( var item in dropsTemp )
            {
                onDrop?.Invoke( new IInventory.DropEventInfo()
                {
                    Amount = 1,
                    Item = item,
                    SlotOrigin = 0,
                    Self = this
                } );
            }
        }

        public bool IsValidIndex( int slotIndex, Item item )
        {
            throw new System.NotImplementedException();
        }

        public List<int> GetAllValidIndices()
        {
            throw new System.NotImplementedException();
        }
        public (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack )
        {
            throw new System.NotImplementedException();
        }

        public (ItemStack, int orig) GetItemSlot( int slotIndex )
        {
            throw new System.NotImplementedException();
        }

        public List<(ItemStack, int orig)> GetItemSlots()
        {
            throw new System.NotImplementedException();
        }

        public int? CanFit( ItemStack itemStack, int slotIndex )
        {
            throw new System.NotImplementedException();
        }

        public int TryAdd( ItemStack itemStack )
        {
            throw new System.NotImplementedException();
        }

        public int SetItem( ItemStack itemStack, int slotIndex )
        {
            throw new System.NotImplementedException();
        }

        public int? CanRemove( int slotIndex )
        {
            throw new System.NotImplementedException();
        }

        public int TryRemove( ItemStack itemStack )
        {
            throw new System.NotImplementedException();
        }

        public int TryRemove( int amount, int slotIndex )
        {
            throw new System.NotImplementedException();
        }
    }
}