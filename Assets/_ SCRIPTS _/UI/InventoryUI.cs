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

        [SerializeField] private float slotSize = 50.0f;

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

            rt.anchoredPosition = new Vector2( x * slotSize, y * -slotSize );
            rt.sizeDelta = new Vector2( slotSize, slotSize );
        }

        private void SpawnNew( Inventory.PickupEventInfo e )
        {
            GameObject go = Instantiate( itemUIPrefab, itemContainer );
            ItemUI itemUI = go.GetComponent<ItemUI>();
            itemUI.Inventory = playerInv;
            itemUI.Slot = e.OriginSlot;
            itemUI.SetAmount( e.Amount );

            Texture2D tex = RenderTextureManager.GetTexture( e.Item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;
            // slot offset + center
            float x = (e.OriginSlot.x * slotSize) + ((e.Item.Size.x * slotSize) * 0.5f);
            float y = (e.OriginSlot.y * -slotSize) + ((e.Item.Size.y * -slotSize) * 0.5f);

            rt.anchoredPosition = new Vector2( x, y );

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
#warning todo - if the item was added to the count, don't spawn new.
        }

        public void OnDrop( Inventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.OriginSlot].gameObject );

            itemUIs.Remove( e.OriginSlot );
        }
    }
}