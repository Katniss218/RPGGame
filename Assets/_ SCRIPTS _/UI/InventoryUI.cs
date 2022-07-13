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

        protected Dictionary<int, InventorySlotUI> slotUIs = new Dictionary<int, InventorySlotUI>();
        protected Dictionary<int, InventoryItemUI> itemUIs = new Dictionary<int, InventoryItemUI>();

        public const float SLOT_SIZE = 40.0f;
        public const float SLOT_ITEM_SIZE = 50.0f;

        protected override void Awake()
        {
            Ensure.NotNull( slotContainer );
            Ensure.NotNull( itemContainer );

            base.Awake();
        }

        void OnEnable()
        {
            RedrawInventory();
        }

        public abstract void SetSlotUIPositionAndScale( RectTransform transform, int slotIndex );

        public abstract void SetItemUIPosition( RectTransform transform, int slotIndex, Item item );

        public abstract Vector2 GetItemSize( int slotIndex, Item item );

        public abstract void RedrawInventory();

        protected void SpawnSlot( int index )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_slot" ), slotContainer );
            RectTransform rt = (RectTransform)go.transform;

            InventorySlotUI slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.Inventory = this.Inventory;
            slotUI.Slot = index;

            SetSlotUIPositionAndScale( rt, index );

            slotUIs.Add( index, slotUI );
        }

        protected void SpawnItem( Item item, int amount, int slotIndex )
        {
            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/UI/inventory_item" ), itemContainer );
            InventoryItemUI itemUI = go.GetComponent<InventoryItemUI>();
            itemUI.SetAmount( amount );

            Texture2D tex = RenderedIconManager.GetTexture( item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            RectTransform rt = (RectTransform)go.transform;

            SetItemUIPosition( rt, slotIndex, item );
            itemUI.SetIconSize( GetItemSize( slotIndex, item ) );

            itemUIs.Add( slotIndex, itemUI );
        }

        protected void UpdateItem( Item item, int amount, int slotIndex )
        {
            InventoryItemUI itemUI = itemUIs[slotIndex];

            Texture2D tex = RenderedIconManager.GetTexture( item.ID );
            Sprite sprite = Sprite.Create( tex, new Rect( 0, 0, tex.width, tex.height ), Vector2.zero );

            itemUI.SetIcon( sprite );

            itemUI.SetAmount( amount );
        }

        public virtual void OnPickup( IInventory.PickupEventInfo e )
        {
            if( slotUIs.Count == 0 )
            {
                return;
            }
            if( itemUIs.ContainsKey( e.SlotOrigin ) )
            {
                (ItemStack item, _) = e.Self.GetItemSlot( e.SlotOrigin );
                UpdateItem( e.Item, item.Amount, e.SlotOrigin );
            }
            else
            {
                SpawnItem( e.Item, e.Amount, e.SlotOrigin );
            }
        }

        public virtual void OnDrop( IInventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.SlotOrigin].gameObject );

            itemUIs.Remove( e.SlotOrigin );
        }

        public virtual void OnResize( IInventory.ResizeEventInfo e )
        {
            RedrawInventory();
        }
    }
}