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

            if( !itemStack.IsEmpty ) // pick up to hand
            {
                if( !ItemDragAndDrop.cursorItem.IsEmpty ) // hand is not empty
                {
                    return;
                }

                Texture2D tex = RenderedIconManager.GetTexture( itemStack.Item.ID );

                Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
                ItemDragAndDrop.Instance.SetIcon( sprite );
                ItemDragAndDrop.Instance.SetAmount( itemStack.Amount );

                float texWorldSize = RenderedIconManager.GetTextureWorldSize( itemStack.Item.ID );
                ItemDragAndDrop.Instance.SetIconSize( new Vector2( texWorldSize * PlayerInventoryUI.SLOT_ITEM_SIZE, texWorldSize * PlayerInventoryUI.SLOT_ITEM_SIZE ) );

                ItemDragAndDrop.cursorItem = itemStack.Copy();
                Inventory.RemoveItem( itemStack.Amount, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
            }
            else // drop from hand
            {
                if( ItemDragAndDrop.cursorItem.IsEmpty ) // hand is empty
                {
                    return;
                }

                int? canFit = Inventory.CanSetItem( ItemDragAndDrop.cursorItem, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
                if( canFit == null )
                {
                    return;
                }
                // TODO - can fit partially.
                if( canFit < itemStack.Amount )
                {
                    return;
                }
                // can fit entire stack.
                ItemDragAndDrop.Instance.SetIcon( null );
                ItemDragAndDrop.Instance.SetAmount( null );

                Inventory.SetItem( ItemDragAndDrop.cursorItem, orig, IInventory.Reason.INVENTORY_REARRANGEMENT );
                ItemDragAndDrop.cursorItem.MakeEmpty();
            }
        }
    }
}