using RPGGame.Items;
using RPGGame.Items.Inventories;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI.Windows
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : GridInventoryUI
    {
        private static Vector2[] equipSlotPositions = new Vector2[]
        {
            new Vector2( 20, 210 ),  // mainhand
            new Vector2( 160, 210 ), // offhand
            new Vector2( 90, 270 ), // head
            new Vector2( 90, 210 ), // chest
            new Vector2( 90, 150 ), // legs
            new Vector2( 90, 90 )  // feet
        };

        protected override void Awake()
        {
            base.Awake();
        }

        public static PlayerInventoryUI CreateUIWindow( PlayerInventory inventory, Transform owner )
        {
            (RectTransform rt, PlayerInventoryUI invUI) = UIWindow.Create<PlayerInventoryUI>( "Player Inventory", owner, Main.UIWindowCanvas );

            float windowSizeX = Mathf.Max((inventory.SizeX * SLOT_SIZE + 10f + 10f), 200f);
            float windowSizeY = (inventory.SizeY * SLOT_SIZE + 10f + 10f) + 300f;

            rt.ApplyTransformUI( Vector2.one, Vector2.one, Vector2.zero, new Vector2( windowSizeX, windowSizeY ) );

            RectTransform slotContainer = GameObjectEx.CreateUI( "Slot Container", rt );
            slotContainer.ApplyTransformUI( new Vector2( 0.5f, 0.0f ), 10, 10, 310, 10 );

            RectTransform itemContainer = GameObjectEx.CreateUI( "Item Container", rt );
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