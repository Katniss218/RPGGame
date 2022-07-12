using RPGGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public abstract class InventoryUI<T> : UIWindow where T : IInventory
    {
        public T Inventory;

        [SerializeField] protected RectTransform slotContainer;
        [SerializeField] protected RectTransform itemContainer;

        Dictionary<int, InventoryItemUI> itemUIs = new Dictionary<int, InventoryItemUI>();

        public const float SLOT_SIZE = 40.0f;
        public const float SLOT_ITEM_SIZE = 50.0f;

        protected override void Awake()
        {
            Ensure.NotNull( slotContainer );
            Ensure.NotNull( itemContainer );

            base.Awake();
        }

        protected override void Start()
        {
            RedrawSlots();

            base.Start();
        }

        public abstract Vector2 GetSlotPosition( int slotIndex );

        public abstract Vector2 GetItemPosition( int slotIndex, Item item );

        public abstract Vector2 GetItemSize( int slotIndex, Item item );

        public abstract void RedrawSlots();

        protected void SpawnSlot( int index )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_slot" ), slotContainer );
            RectTransform rt = (RectTransform)go.transform;

            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.Inventory = this.Inventory;
            slotUI.Slot = index;

            rt.anchoredPosition = GetSlotPosition( index );
            rt.sizeDelta = new Vector2( SLOT_SIZE, SLOT_SIZE );
        }

        protected void SpawnItem( IInventory.PickupEventInfo e )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_item" ), itemContainer );
            InventoryItemUI itemUI = go.GetComponent<InventoryItemUI>();
            itemUI.SetAmount( e.Amount );

            Texture2D tex = RenderedIconManager.GetTexture( e.Item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;

            rt.anchoredPosition = GetItemPosition( e.SlotOrigin, e.Item );
            itemUI.SetIconSize( GetItemSize( e.SlotOrigin, e.Item ) );

            itemUIs.Add( e.SlotOrigin, itemUI );
        }

        protected void UpdateItem( IInventory.PickupEventInfo e )
        {
            (ItemStack itemStack, _) = e.Self.GetItemSlot( e.SlotOrigin );

            InventoryItemUI itemUI = itemUIs[e.SlotOrigin];
            itemUI.SetAmount( itemStack.Amount );
        }

        public virtual void OnPickup( IInventory.PickupEventInfo e )
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

        public virtual void OnDrop( IInventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.SlotOrigin].gameObject );

            itemUIs.Remove( e.SlotOrigin );
        }

        public virtual void OnResize( IInventory.ResizeEventInfo e )
        {
            RedrawSlots();
        }
    }
}