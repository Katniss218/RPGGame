using RPGGame.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : GridInventoryUI
    {
        private static Vector2[] equipSlotPositions = new Vector2[]
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
            base.Awake();
        }

        public static PlayerInventoryUI CreateUIWindow( PlayerInventory inventory )
        {
            (RectTransform rt, PlayerInventoryUI invUI) = UIWindow.Create<PlayerInventoryUI>( "Player Inventory", Main.UIWindowCanvas );

            float windowSizeX = Mathf.Max((inventory.SizeX * SLOT_SIZE + 10f + 10f), 200f);
            float windowSizeY = (inventory.SizeY * SLOT_SIZE + 10f + 10f) + 300f;

            rt.ApplyTransformUI( Vector2.one, Vector2.one, Vector2.zero, new Vector2( windowSizeX, windowSizeY ) );

            RectTransform slotContainer = GameObjectUtils.CreateUI( "Slot Container", rt );
            slotContainer.ApplyTransformUI( new Vector2( 0.5f, 0.0f ), 10, 10, 310, 10 );

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
            RectTransform transform = (RectTransform)slotUI.transform;

            if( slotIndex < 0 )
            {
                int equipIndex = PlayerInventory.MapSlotIndexToEquipIndex( slotIndex );
                transform.anchoredPosition = equipSlotPositions[equipIndex];
                transform.sizeDelta = new Vector2( 60, 60 );
                return;
            }

            base.SetSlotUIPositionAndScale( slotUI, slotIndex );
        }

        public override void SetItemUIPosition( InventoryItemUI itemUI, int slotIndex, Item item )
        {
            RectTransform transform = (RectTransform)itemUI.transform;

            if( slotIndex < 0 )
            {
                transform.MoveOverCentered( (RectTransform)slotUIs[slotIndex].transform );
                return;
            }

            base.SetItemUIPosition( itemUI, slotIndex, item );
        }

        public override void SetItemSize( InventoryItemUI itemUI, int slotIndex, Item item )
        {
            // some mapping with other slots here.

            base.SetItemSize( itemUI, slotIndex, item );
        }
    }
}