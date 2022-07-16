using RPGGame.Audio;
using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    [RequireComponent( typeof( Button ) )]
    public class InventorySlotUI : MonoBehaviour
    {
        private Button btn;

        public IInventory Inventory;

        public int Slot;

        void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener( OnClick );
        }

        private void OnClick()
        {
            (ItemStack slotItem, int slotIndex) = Inventory.GetItemSlot( Slot );

            if( !slotItem.IsEmpty )
            {
                if( !ItemDragAndDrop.cursorItem.IsEmpty ) // both non-empty
                {
                    if( slotItem.CanStackWith( ItemDragAndDrop.cursorItem ) )
                    {
                        // add to the stack.
                        FullHandToFullSlot( slotItem, slotIndex );
                    }
                    return;
                }

                // slot non-empty, cursor empty.

                // pick up to hand
                FullSlotToEmptyHand( slotItem, slotIndex );
            }
            else
            {
                if( ItemDragAndDrop.cursorItem.IsEmpty ) // both empty
                {
                    return;
                }

                // slot empty, cursor non-empty.

                int? canFit = Inventory.CanSetItem( ItemDragAndDrop.cursorItem, slotIndex, IInventory.Reason.INVENTORY_REARRANGEMENT );
                if( canFit == null || canFit < slotItem.Amount )
                {
                    return;
                }
                // can fit entire stack.
                FullHandToEmptySlot( slotItem, slotIndex );
            }
        }

        private void FullHandToEmptySlot( ItemStack slotItem, int slotIndex )
        {
            ItemDragAndDrop.Instance.SetIcon( null );
            ItemDragAndDrop.Instance.SetAmount( null );

            Inventory.AddItem( ItemDragAndDrop.cursorItem, slotIndex, IInventory.Reason.INVENTORY_REARRANGEMENT );
            ItemDragAndDrop.cursorItem.MakeEmpty();
            AudioManager.PlaySound( slotItem.Item.DropSound );
        }

        private void FullSlotToEmptyHand( ItemStack slotItem, int slotIndex )
        {
            Texture2D tex = RenderedIconManager.GetTexture( slotItem.Item.ID );

            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
            ItemDragAndDrop.Instance.SetIcon( sprite );
            ItemDragAndDrop.Instance.SetAmount( slotItem.Amount );

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( slotItem.Item.ID );
            ItemDragAndDrop.Instance.SetIconSize( new Vector2( texWorldSize * InventoryUI.SLOT_ITEM_SIZE, texWorldSize * InventoryUI.SLOT_ITEM_SIZE ) );

            ItemDragAndDrop.cursorItem = slotItem.Copy();
            AudioManager.PlaySound( slotItem.Item.PickupSound );
            Inventory.RemoveItem( slotItem.Amount, slotIndex, IInventory.Reason.INVENTORY_REARRANGEMENT );
        }

        private void FullHandToFullSlot( ItemStack slotItem, int slotIndex )
        {
            AudioClip sound = slotItem.Item.DropSound;
            int amountAdded = Inventory.AddItem( ItemDragAndDrop.cursorItem, slotIndex, IInventory.Reason.INVENTORY_REARRANGEMENT );
            if( amountAdded > 0 )
            {
                ItemDragAndDrop.cursorItem.Sub( amountAdded );
                AudioManager.PlaySound( sound );
                if( ItemDragAndDrop.cursorItem.IsEmpty )
                {
                    ItemDragAndDrop.Instance.SetIcon( null );
                    ItemDragAndDrop.Instance.SetAmount( null );
                }
            }
        }
    }
}