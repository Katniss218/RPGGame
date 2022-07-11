using RPGGame.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    [DisallowMultipleComponent]
    public class PlayerInventoryUI : UIWindow
    {
        public PlayerInventory Inventory;
        [SerializeField] private RectTransform slotContainer;
        [SerializeField] private RectTransform itemContainer;

        Dictionary<int, InventoryItemUI> itemUIs = new Dictionary<int, InventoryItemUI>();

        public const float SLOT_SIZE = 40.0f;
        public const float SLOT_ITEM_SIZE = 50.0f;

        protected override void Awake()
        {
            Ensure.NotNull( Inventory );
            Ensure.NotNull( slotContainer );
            Ensure.NotNull( itemContainer );

            base.Awake();
        }

        protected override void Start()
        {
            RedrawSlots();

            base.Start();
        }

        public void RedrawSlots()
        {
            bool[,] slotMask = Inventory.GetBlockingSlotMask();
            for( int y = 0; y < Inventory.InvSizeY; y++ )
            {
                for( int x = 0; x < Inventory.InvSizeX; x++ )
                {
                    if( !slotMask[x, y] )
                    {
                        SpawnSlot( x, y );
                    }
                }
            }
        }

        private void SpawnSlot( int posX, int posY )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_slot" ), slotContainer );
            RectTransform rt = (RectTransform)go.transform;

            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.InventoryUI = this;
            slotUI.Slot = Items.Inventory.MapIndexSlot( posX, posY, Inventory.InvSizeX );

            rt.anchoredPosition = new Vector2( posX * SLOT_SIZE, posY * -SLOT_SIZE );
            rt.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

        private void SpawnItem( Inventory.PickupEventInfo e )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_item" ), itemContainer );
            InventoryItemUI itemUI = go.GetComponent<InventoryItemUI>();
            itemUI.SetAmount( e.Amount );

            Texture2D tex = RenderedIconManager.GetTexture( e.Item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;
            // slot offset + center
            float x = (Items.Inventory.MapSlotIndexCoord( e.SlotOrigin, Inventory.InvSizeX ).x * SLOT_SIZE) + ((e.Item.Size.x * SLOT_SIZE) * 0.5f);
            float y = (Items.Inventory.MapSlotIndexCoord( e.SlotOrigin, Inventory.InvSizeX ).y * -SLOT_SIZE) + ((e.Item.Size.y * -SLOT_SIZE) * 0.5f);

            rt.anchoredPosition = new Vector2( x, y );

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( e.Item.ID );
            itemUI.SetIconSize( new Vector2( texWorldSize * SLOT_ITEM_SIZE, texWorldSize * SLOT_ITEM_SIZE ) );

            itemUIs.Add( e.SlotOrigin, itemUI );
        }

        private void UpdateItem( Inventory.PickupEventInfo e )
        {
            (_, int amount, _) = e.Self.GetItemSlot( e.SlotOrigin );

            InventoryItemUI itemUI = itemUIs[e.SlotOrigin];
            itemUI.SetAmount( amount );
        }

        public void OnResize( Inventory.ResizeEventInfo e )
        {
            RedrawSlots();
        }

        public void OnPickup( Inventory.PickupEventInfo e )
        {
            if( itemUIs.ContainsKey( e.SlotOrigin ) )
            {
                UpdateItem( e );
            }
            else
            {
                SpawnItem( e );
            }
        }

        public void OnDrop( Inventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.SlotOrigin].gameObject );

            itemUIs.Remove( e.SlotOrigin );
        }
    }
}