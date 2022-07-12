using RPGGame.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : InventoryUI<PlayerInventory>
    {
        [SerializeField] private RectTransform[] slotUIs;
        [SerializeField] private Transform equipSlotContainer;

        protected override void Awake()
        {
            Ensure.NotNull( Inventory );

            base.Awake();
        }

        public static Vector2 SwitchToRectTransformCentered( RectTransform from, RectTransform to )
        {
            // null cameras because this assumed canvas with 'screen space - overlay'.

            // I'd need to spawn the object you want to move, and then use this.
            // It'll get you the position that'll make the center of 'to' directly over the center of 'from'.
            Vector2 fromPivotDerivedOffset = new Vector2( from.rect.width * 0.5f + from.rect.xMin, from.rect.height * 0.5f + from.rect.yMin ); // (0, -30) // this applies offset to move it to the middle.
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint( null, from.position ); // (1720.00, 837.00)
            screenP += fromPivotDerivedOffset; // (1720.00, 807.00)

            RectTransformUtility.ScreenPointToLocalPointInRectangle( to, screenP, null, out Vector2 localPoint ); // (50.00, -90.00)
            Vector2 pivotDerivedOffset = new Vector2( to.rect.width * 0.5f + to.rect.xMin, to.rect.height * 0.5f + to.rect.yMin ); // (0, 0)

            return to.anchoredPosition + localPoint - pivotDerivedOffset; // (0, 0) + (50, 90) + (0, 0)
        }

        public static Vector2 SwitchToRectTransform( RectTransform from, RectTransform to )
        {
            // This one returns the position that'll make the pivot of 'to' be directly over pivot of 'from's.

            Vector2 fromPivotDerivedOffset = new Vector2( from.rect.width * from.pivot.x + from.rect.xMin, from.rect.height * from.pivot.y + from.rect.yMin ); // xmin = -30 ymin = -60 h/w = 60
            Vector2 screenP = RectTransformUtility.WorldToScreenPoint( null, from.position );
            screenP += fromPivotDerivedOffset;

            RectTransformUtility.ScreenPointToLocalPointInRectangle( to, screenP, null, out Vector2 localPoint );
            Vector2 pivotDerivedOffset = new Vector2( to.rect.width * to.pivot.x + to.rect.xMin, to.rect.height * to.pivot.y + to.rect.yMin );

            return to.anchoredPosition + localPoint - pivotDerivedOffset;
        }

        public override Vector2 GetSlotPosition( int slotIndex )
        {
            if( slotIndex < 0 )
            {
                throw new System.Exception( "EquipSlots are meant to be spawned manually." );
            }
#warning TODO - add references to each slot position and do it based on this.
            
            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.InvSizeX );

            return new Vector2(
                x * SLOT_SIZE,
                y * -SLOT_SIZE );
        }

        const float VERTICAL_OFFSET = -300f;

        public override Vector2 GetItemPosition( RectTransform transform, int slotIndex, Item item )
        {
            // 'transform' is needed to get the pivot and other things.
            if( slotIndex < 0 )
            {
                int equipIndex = Inventory.MapSlotIndexToEquipIndex( slotIndex );

                return SwitchToRectTransform( slotUIs[equipIndex], transform );
            }

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.InvSizeX );

            return new Vector2(
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