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
        public PlayerInventoryUI InventoryUI;

        public Inventory Inventory { get => InventoryUI.Inventory; }

        public int Slot;

#warning TODO - the inventory UI should provide a way of mapping slot indices to display positions.

        void Awake()
        {
            btn = GetComponent<Button>();
            btn.onClick.AddListener( OnClick );
        }

        private void OnClick()
        {
            (Item i, int amt, int orig) = Inventory.GetItemSlot( Slot );

            if( i != null ) // pick up to hand
            {
                if( ItemDragAndDrop.cursorItem.i != null ) // hand is not empty
                {
                    return;
                }

                Texture2D tex = RenderedIconManager.GetTexture( i.ID );

                Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );
                ItemDragAndDrop.Instance.SetIcon( sprite );
                ItemDragAndDrop.Instance.SetAmount( amt );

                float texWorldSize = RenderedIconManager.GetTextureWorldSize( i.ID );
                ItemDragAndDrop.Instance.SetIconSize( new Vector2( texWorldSize * PlayerInventoryUI.SLOT_ITEM_SIZE, texWorldSize * PlayerInventoryUI.SLOT_ITEM_SIZE ) );


                ItemDragAndDrop.cursorItem = (i, amt);
#warning TODO - This might lead to lost items if something drops from your inventory. The event callback creating pickups on player inv was removed because it was creating pickups when I dropped the items here.
                Inventory.Drop( amt, orig );
            }
            else // drop from hand
            {
                if( ItemDragAndDrop.cursorItem.i == null ) // hand is empty
                {
                    return;
                }

                int? canFit = Inventory.CanPickUp( ItemDragAndDrop.cursorItem.i, orig );
                if( canFit == null )
                {
                    return;
                }
                // TODO - can fit partially.
                if( canFit < amt )
                {
                    return;
                }
                // can fit entire stack.
                ItemDragAndDrop.Instance.SetIcon( null );
                ItemDragAndDrop.Instance.SetAmount( null );

                Inventory.PickUp( ItemDragAndDrop.cursorItem.i, ItemDragAndDrop.cursorItem.amt, orig );
                ItemDragAndDrop.cursorItem = (null, 0);
            }
        }
    }
}