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

        public Vector2 GetCenter( int slotIndex, Vector2Int itemSize )
        {
            // some mapping with other slots here.

            (int x, int y) = GridInventory.GetSlotCoords( slotIndex, Inventory.InvSizeX );

            return new Vector2(
                x * SLOT_SIZE + ((itemSize.x * SLOT_SIZE) * 0.5f),
                y * -SLOT_SIZE + ((itemSize.y * -SLOT_SIZE) * 0.5f) );
        }

        public void RedrawSlots()
        {
            bool[] slotMask = Inventory.GetBlockingSlotMask();

            for( int i = 0; i < Inventory.InvSizeX * Inventory.InvSizeY; i++ )
            {
                if( !slotMask[i] )
                {
                    SpawnSlot( i );
                }
            }
        }

        private void SpawnSlot( int index )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_slot" ), slotContainer );
            RectTransform rt = (RectTransform)go.transform;

            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.InventoryUI = this;
            slotUI.Slot = index;

            (int x, int y) = GridInventory.GetSlotCoords( index, Inventory.InvSizeX );
            rt.anchoredPosition = new Vector2( x * SLOT_SIZE, y * -SLOT_SIZE );
            rt.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

        private void SpawnItem( IInventory.PickupEventInfo e )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_item" ), itemContainer );
            InventoryItemUI itemUI = go.GetComponent<InventoryItemUI>();
            itemUI.SetAmount( e.Amount );

            Texture2D tex = RenderedIconManager.GetTexture( e.Item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;

            rt.anchoredPosition = GetCenter( e.SlotOrigin, e.Item.Size );

            float texWorldSize = RenderedIconManager.GetTextureWorldSize( e.Item.ID );
            itemUI.SetIconSize( new Vector2( texWorldSize * SLOT_ITEM_SIZE, texWorldSize * SLOT_ITEM_SIZE ) );

            itemUIs.Add( e.SlotOrigin, itemUI );
        }

        private void UpdateItem( IInventory.PickupEventInfo e )
        {
            (_, int amount, _) = e.Self.GetItemSlot( e.SlotOrigin );

            InventoryItemUI itemUI = itemUIs[e.SlotOrigin];
            itemUI.SetAmount( amount );
        }

        public void OnPickup( IInventory.PickupEventInfo e )
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

        public void OnDrop( IInventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.SlotOrigin].gameObject );

            itemUIs.Remove( e.SlotOrigin );
        }

        public void OnResize( IInventory.ResizeEventInfo e )
        {
            RedrawSlots();
        }
    }
}