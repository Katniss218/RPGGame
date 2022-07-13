using RPGGame.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : InventoryUI<PlayerInventory>
    {
        private Vector2[] equipSlotPositions = new Vector2[]
        {
            new Vector2( 20, 240 ),  // mainhand
            new Vector2( 160, 240 ), // offhand
            new Vector2( 90, 300 ), // head
            new Vector2( 90, 240 ), // chest
            new Vector2( 90, 180 ), // legs
            new Vector2( 90, 120 )  // feet
        };

        protected override void Awake()
        {
            Ensure.NotNull( Inventory );

            base.Awake();
        }

        public override void SetSlotUIPositionAndScale( RectTransform transform, int slotIndex )
        {
            if( slotIndex < 0 )
            {
                int equipIndex = Inventory.MapSlotIndexToEquipIndex( slotIndex );
                transform.anchoredPosition = equipSlotPositions[equipIndex];
                transform.sizeDelta = new Vector2( 60, 60 );
                return;
            }

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.SizeX );

            transform.anchoredPosition = new Vector2(
                x * SLOT_SIZE,
                y * -SLOT_SIZE );
            transform.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

        const float VERTICAL_OFFSET = -300f;

        public override void SetItemUIPosition( RectTransform transform, int slotIndex, Item item )
        {
            if( slotIndex < 0 )
            {
                transform.MoveOver( (RectTransform)slotUIs[slotIndex].transform );
                return;
            }

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.SizeX );

            transform.anchoredPosition = new Vector2(
                x * SLOT_SIZE + ((item.Size.x * SLOT_SIZE) * 0.5f),
                y * -SLOT_SIZE + ((item.Size.y * -SLOT_SIZE) * 0.5f) + VERTICAL_OFFSET );
        }

        public override Vector2 GetItemSize( int slotIndex, Item item )
        {
            // some mapping with other slots here.

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( item.ID );

            return new Vector2(
               texWorldSize * SLOT_ITEM_SIZE,
               texWorldSize * SLOT_ITEM_SIZE );
        }

        public override void DrawInventory()
        {
            List<int> slotIndices = Inventory.GetAllValidIndices();

            foreach( int index in slotIndices )
            {
                SpawnSlot( index );
            }

            List<(ItemStack, int orig)> items = Inventory.GetItemSlots();

            foreach( var (item, orig) in items)
            {
                if( itemUIs.ContainsKey( orig ) )
                {
                    UpdateItem( item.Item, item.Amount, orig );
                }
                else
                {
                    SpawnItem( item.Item, item.Amount, orig );
                }
            }
        }
    }
}