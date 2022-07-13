using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class GridInventoryUI : InventoryUI
    {
        public override void SetSlotUIPositionAndScale( InventorySlotUI slotUI, int slotIndex )
        {
            GridInventory inv = (GridInventory)this.Inventory;
            RectTransform transform = (RectTransform)slotUI.transform;

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, inv.SizeX );

            transform.anchoredPosition = new Vector2(
                x * SLOT_SIZE,
                y * -SLOT_SIZE );
            transform.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

#warning TODO - I don't like this vertical offset thing.
        const float VERTICAL_OFFSET = -300f;

        public override void SetItemUIPosition( InventoryItemUI itemUI, int slotIndex, Item item )
        {
            GridInventory inv = (GridInventory)this.Inventory;
            RectTransform transform = (RectTransform)itemUI.transform;

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, inv.SizeX );

            transform.anchoredPosition = new Vector2(
                x * SLOT_SIZE + ((item.Size.x * SLOT_SIZE) * 0.5f),
                y * -SLOT_SIZE + ((item.Size.y * -SLOT_SIZE) * 0.5f) + VERTICAL_OFFSET );
        }

        public override void SetItemSize( InventoryItemUI itemUI, int slotIndex, Item item )
        {
            // some mapping with other slots here.

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( item.ID );

            Vector2 newSize = new Vector2(
               texWorldSize * SLOT_ITEM_SIZE,
               texWorldSize * SLOT_ITEM_SIZE );

            itemUI.SetIconSize( newSize );
        }
    }
}