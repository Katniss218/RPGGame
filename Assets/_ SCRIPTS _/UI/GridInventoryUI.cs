using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class GridInventoryUI : InventoryUI
    {

        public static GridInventoryUI CreateUIWindow( GridInventory inventory )
        {
            (RectTransform rt, GridInventoryUI invUI) = UIWindow.Create<GridInventoryUI>( "Grid Inventory", Main.UIWindowCanvas );

            float windowSizeX = inventory.SizeX * SLOT_SIZE + 10f + 10f;
            float windowSizeY = inventory.SizeY * SLOT_SIZE + 10f + 40f;

            rt.ApplyTransformUI( Vector2.up, Vector2.up, Vector2.zero, new Vector2( windowSizeX, windowSizeY ) );

            RectTransform slotContainer = GameObjectUtils.CreateUI( "Slot Container", rt );
            slotContainer.ApplyTransformUI( new Vector2( 0.5f, 0.0f ), 10, 10, 40, 10 );

            RectTransform itemContainer = GameObjectUtils.CreateUI( "Item Container", rt );
            itemContainer.ApplyTransformUI( new Vector2( 0.5f, 0.5f ), 0f, 0f, 0f, 0f );

            invUI.slotContainer = slotContainer;
            invUI.itemContainer = itemContainer;

            invUI.Inventory = inventory;

            inventory.onPickup.AddListener( invUI.OnPickup );
            inventory.onDrop.AddListener( invUI.OnDrop );
            inventory.onResize.AddListener( invUI.OnResize );

            invUI.Redraw();

            return invUI;
        }


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

        public override void SetItemUIPosition( InventoryItemUI itemUI, int slotIndex, Item item )
        {
            RectTransform transform = (RectTransform)itemUI.transform;

            transform.MoveOverCentered( (RectTransform)slotUIs[slotIndex].transform );
            transform.anchoredPosition += new Vector2( (item.Size.x - 1) * SLOT_SIZE * 0.5f, (item.Size.y - 1) * -SLOT_SIZE * 0.5f );
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