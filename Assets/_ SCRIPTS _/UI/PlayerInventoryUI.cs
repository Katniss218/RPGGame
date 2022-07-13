using RPGGame.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : GridInventoryUI
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
#warning TODO - remove when it's spawned by hand.
            Inventory = FindObjectOfType<PlayerInventory>();

            Ensure.NotNull( Inventory );

            base.Awake();
        }

        public static RectTransform CreateUIWindow( PlayerInventory inventory )
        {
            throw new NotImplementedException();
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

        public override void SetItemSize(InventoryItemUI itemUI, int slotIndex, Item item )
        {
            // some mapping with other slots here.

            base.SetItemSize( itemUI, slotIndex, item );
        }
    }
}