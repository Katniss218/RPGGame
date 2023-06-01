using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items.Inventories
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [DisallowMultipleComponent]
    public class MobInventory : MonoBehaviour, IInventory
    {
        public Item EquipHand;

        public Item[] Drops;

        [field: SerializeField]
        public UnityEvent<IInventory.PickupEventInfo> onPickup { get; private set; }

        [field: SerializeField]
        public UnityEvent<IInventory.DropEventInfo> onDrop { get; private set; }

        [field: SerializeField]
        public UnityEvent<IInventory.ResizeEventInfo> onResize { get; private set; }

        public bool IsValidIndex( int slotIndex, Item item )
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<int> GetAllSlots()
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

        public (List<(int index, int amt)>, int leftover) GetNeededSlots( ItemStack itemStack, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
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

        public IEnumerable<(ItemStack, int orig)> GetItemSlots()
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

        public int? CanAddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            throw new System.NotImplementedException();
        }

        public int AddItem( ItemStack itemStack, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            return 0;
            throw new System.NotImplementedException();
        }

        public int? CanRemoveItem( int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            throw new System.NotImplementedException();
        }

        public int RemoveItem( int amount, int slotIndex, IInventory.ChangeReason reason = IInventory.ChangeReason.GENERIC )
        {
            throw new System.NotImplementedException();
        }

        //  ---------------------

        //      SERIALIZATION
        //

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