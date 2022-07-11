using RPGGame.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : InventoryUI<PlayerInventory>
    {
        [SerializeField] private Transform equipSlotContainer;

        protected override void Awake()
        {
            Ensure.NotNull( Inventory );

            base.Awake();
        }

        public override Vector2 GetSlotPosition( int slotIndex )
        {
            if( slotIndex == -1 )
            {
                return new Vector2( 500, -500 );
            }
            // some mapping with other slots here.

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.InvSizeX );

            return new Vector2(
                x * SLOT_SIZE,
                y * -SLOT_SIZE );
        }

        public override Vector2 GetItemPosition( int slotIndex, Item item )
        {
            if( slotIndex == -1 )
            {
                return new Vector2( 50, 215 );
            }
            // some mapping with other slots here.

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.InvSizeX );

            return new Vector2(
                x * SLOT_SIZE + ((item.Size.x * SLOT_SIZE) * 0.5f),
                y * -SLOT_SIZE + ((item.Size.y * -SLOT_SIZE) * 0.5f) );
        }

        public override Vector2 GetItemSize( int slotIndex, Item item )
        {
            // some mapping with other slots here.

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( item.ID );

            return new Vector2(
               texWorldSize * SLOT_ITEM_SIZE,
               texWorldSize * SLOT_ITEM_SIZE );
        }

        public override void RedrawSlots()
        {
            InventorySlotUI[] comp = equipSlotContainer.GetComponentsInChildren<InventorySlotUI>();
            foreach( var s in comp )
            {
                s.Inventory = this.Inventory;
            }

            bool[] slotMask = Inventory.GetBlockingSlotMask();

            for( int i = 0; i < Inventory.InvSizeX * Inventory.InvSizeY; i++ )
            {
                if( !slotMask[i] )
                {
                    SpawnSlot( i );
                }
            }
        }
    }
}