using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInv;
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private RectTransform itemContainer;

        [SerializeField] private GameObject inventorySlotUIPrefab;
        [SerializeField] private GameObject itemUIPrefab;

        Dictionary<Vector2Int, ItemUI> itemUIs = new Dictionary<Vector2Int, ItemUI>();

        private const float SLOT_SIZE = 50.0f;
        private const float SLOT_ITEM_SIZE = 60.0f;

        void Start()
        {
            Ensure.NotNull( playerInv );
            Ensure.NotNull( slotContainer );
            Ensure.NotNull( itemContainer );
            Ensure.NotNull( inventorySlotUIPrefab );
            Ensure.NotNull( itemUIPrefab );

            Redraw();
        }

        public void Redraw()
        {
            bool[,] slotMask = playerInv.GetBlockingSlotMask();
            for( int y = 0; y < playerInv.InvSizeY; y++ )
            {
                for( int x = 0; x < playerInv.InvSizeX; x++ )
                {
                    if( !slotMask[x, y] )
                    {
                        SpawnSlot( x, y );
                    }
                }
            }
        }

        private void SpawnSlot( int x, int y )
        {
            GameObject go = Instantiate( inventorySlotUIPrefab, slotContainer );
            RectTransform rt = (RectTransform)go.transform;

            rt.anchoredPosition = new Vector2( x * SLOT_SIZE, y * -SLOT_SIZE );
            rt.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

        private void SpawnNew( Inventory.PickupEventInfo e )
        {
            GameObject go = Instantiate( itemUIPrefab, itemContainer );
            ItemUI itemUI = go.GetComponent<ItemUI>();
            itemUI.Inventory = playerInv;
            itemUI.Slot = e.OriginSlot;
            itemUI.SetAmount( e.Amount );

            Texture2D tex = RenderTextureManager.GetTexture( e.Item.ID );
            float texWorldSize = RenderTextureManager.GetTextureWorldSize( e.Item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;
            // slot offset + center
            float x = (e.OriginSlot.x * SLOT_SIZE) + ((e.Item.Size.x * SLOT_SIZE) * 0.5f);
            float y = (e.OriginSlot.y * -SLOT_SIZE) + ((e.Item.Size.y * -SLOT_SIZE) * 0.5f);

            rt.anchoredPosition = new Vector2( x, y );

            rt.sizeDelta = new Vector2( texWorldSize * SLOT_ITEM_SIZE, texWorldSize * SLOT_ITEM_SIZE );

            itemUIs.Add( e.OriginSlot, itemUI );
        }

        private void UpdateExisting( Inventory.PickupEventInfo e )
        {
            (Item item, int amount) = e.Self.GetItemSlot( e.OriginSlot );

            ItemUI itemUI = itemUIs[e.OriginSlot];
            itemUI.SetAmount( amount );
        }

        public void OnResize( Inventory.ResizeEventInfo e )
        {
            Redraw();
        }

        public void OnPickup( Inventory.PickupEventInfo e )
        {
            if( itemUIs.ContainsKey( e.OriginSlot ) )
            {
                UpdateExisting( e );
            }
            else
            {
                SpawnNew( e );
            }
        }

        public void OnDrop( Inventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.OriginSlot].gameObject );

            itemUIs.Remove( e.OriginSlot );
        }
    }
}