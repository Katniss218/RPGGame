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

        Dictionary<Vector2Int, ItemUI> itemUIs = new Dictionary<Vector2Int, ItemUI>();

        private const float SLOT_SIZE = 50.0f;
        private const float SLOT_ITEM_SIZE = 60.0f;

        void Start()
        {
            Ensure.NotNull( playerInv );
            Ensure.NotNull( slotContainer );
            Ensure.NotNull( itemContainer );

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
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_slot" ), slotContainer );
            RectTransform rt = (RectTransform)go.transform;

            rt.anchoredPosition = new Vector2( x * SLOT_SIZE, y * -SLOT_SIZE );
            rt.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

        private void SpawnNew( Inventory.PickupEventInfo e )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_item" ), itemContainer );
            ItemUI itemUI = go.GetComponent<ItemUI>();
            itemUI.Inventory = playerInv;
            itemUI.Slot = e.SlotOrigin;
            itemUI.SetAmount( e.Amount );

            Texture2D tex = RenderedIconManager.GetTexture( e.Item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;
            // slot offset + center
            float x = (e.SlotOrigin.x * SLOT_SIZE) + ((e.Item.Size.x * SLOT_SIZE) * 0.5f);
            float y = (e.SlotOrigin.y * -SLOT_SIZE) + ((e.Item.Size.y * -SLOT_SIZE) * 0.5f);

            rt.anchoredPosition = new Vector2( x, y );

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( e.Item.ID );
            itemUI.SetIconSize( new Vector2( texWorldSize * SLOT_ITEM_SIZE, texWorldSize * SLOT_ITEM_SIZE ) );

            itemUIs.Add( e.SlotOrigin, itemUI );
        }

        private void UpdateExisting( Inventory.PickupEventInfo e )
        {
            (_, int amount) = e.Self.GetItemSlot( e.SlotOrigin );

            ItemUI itemUI = itemUIs[e.SlotOrigin];
            itemUI.SetAmount( amount );
        }

        public void OnResize( Inventory.ResizeEventInfo e )
        {
            Redraw();
        }

        public void OnPickup( Inventory.PickupEventInfo e )
        {
            if( itemUIs.ContainsKey( e.SlotOrigin ) )
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
            Destroy( itemUIs[e.SlotOrigin].gameObject );

            itemUIs.Remove( e.SlotOrigin );
        }
    }
}