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
            (ItemStack itemStack, int orig) = Inventory.GetItemSlot( Slot );

            if( !itemStack.IsEmpty )
            {
                if( !ItemDragAndDrop.cursorItem.IsEmpty ) // both non-empty
                {
                    // add to the stack.
                    if (itemStack.CanStackWith( ItemDragAndDrop.cursorItem ))
                    {
                        int amountAdded = Inventory.AddItem( ItemDragAndDrop.cursorItem, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
                        ItemDragAndDrop.cursorItem.Sub( amountAdded );
                        if( ItemDragAndDrop.cursorItem.IsEmpty )
                        {
                            ItemDragAndDrop.Instance.SetIcon( null );
                            ItemDragAndDrop.Instance.SetAmount( null );
                        }
                    }
                    return;
                }

                // pick up to hand

                Texture2D tex = RenderedIconManager.GetTexture( itemStack.Item.ID );

                Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
                ItemDragAndDrop.Instance.SetIcon( sprite );
                ItemDragAndDrop.Instance.SetAmount( itemStack.Amount );

                float texWorldSize = RenderedIconManager.GetTextureWorldSize( itemStack.Item.ID );
                ItemDragAndDrop.Instance.SetIconSize( new Vector2( texWorldSize * PlayerInventoryUI.SLOT_ITEM_SIZE, texWorldSize * PlayerInventoryUI.SLOT_ITEM_SIZE ) );

                ItemDragAndDrop.cursorItem = itemStack.Copy();
                Inventory.RemoveItem( itemStack.Amount, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
            }
            else 
            {
                if( ItemDragAndDrop.cursorItem.IsEmpty ) // both empty
                {
                    return;
                }
                
                // drop from hand

                int? canFit = Inventory.CanSetItem( ItemDragAndDrop.cursorItem, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
                if( canFit == null || canFit < itemStack.Amount )
                {
                    return;
                }
                // can fit entire stack.
                ItemDragAndDrop.Instance.SetIcon( null );
                ItemDragAndDrop.Instance.SetAmount( null );

                Inventory.AddItem( ItemDragAndDrop.cursorItem, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
                ItemDragAndDrop.cursorItem.MakeEmpty();
            }
        }
    }
}