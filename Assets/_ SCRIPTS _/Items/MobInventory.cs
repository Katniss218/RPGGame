using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
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

        public bool IsValidIndex( int slotIndex, Item item )
        {
            throw new System.NotImplementedException();
        }

        public List<int> GetAllSlots()
        {
            List<int> indices = new List<int>();
            if( EquipHand != null )
            {
                indices.Add( -1 );
            }
            for( int i = 0; i < Drops.Length; i++ )
            {
                indices.Add( i );
            }
            return indices;
        }

        public (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack )
        {
            throw new System.NotImplementedException();
        }

        public (ItemStack, int orig) GetItemSlot( int slotIndex )
        {
            if( slotIndex == -1 )
            {
                return (new ItemStack( EquipHand, 1 ), -1);
            }
            return (new ItemStack( Drops[slotIndex], 1 ), slotIndex);
        }

        public List<(ItemStack, int orig)> GetItemSlots()
        {
            List<(ItemStack, int orig)> items = new List<(ItemStack, int orig)>();
            if( EquipHand != null )
            {
                items.Add( (new ItemStack(EquipHand, 1), -1) );
            }
            for( int i = 0; i < Drops.Length; i++ )
            {
                items.Add( (new ItemStack( Drops[i], 1 ), i) );
            }
            return items;
        }

        public int? CanSetItem( ItemStack itemStack, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            throw new System.NotImplementedException();
        }

        public int AddItem( ItemStack itemStack, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            return 0;
            throw new System.NotImplementedException();
        }

        public int? CanRemoveItem( int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            throw new System.NotImplementedException();
        }

        public int RemoveItem( int amount, int slotIndex, IInventory.Reason reason = IInventory.Reason.GENERIC )
        {
            throw new System.NotImplementedException();
        }

        public JObject GetData()
        {
            return IInventoryEx.GetData( this );
        }

        public void SetData( JObject data )
        {
            IInventoryEx.SetData( this, data );
        }
    }
}