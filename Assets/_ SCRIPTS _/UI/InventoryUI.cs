using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private PlayerInventory playerInv;
        [SerializeField] private RectTransform uiContainer;

        [SerializeField] private GameObject itemUIPrefab;

        Dictionary<int, RectTransform> itemUIs = new Dictionary<int, RectTransform>();

        void Start()
        {
            Ensure.NotNull( playerInv );
            Ensure.NotNull( uiContainer );
            Ensure.NotNull( itemUIPrefab );
        }

        public void OnPickup( Inventory.PickupEventInfo e )
        {
            GameObject obj = Instantiate( itemUIPrefab, uiContainer );
            ItemUI itemUI = obj.GetComponent<ItemUI>();
            itemUI.Inventory = playerInv;
            itemUI.Slot = e.Slot;
            itemUI.Text.text = e.Item.DisplayName;

            itemUIs.Add( e.Slot, (RectTransform)obj.transform );
        }

        public void OnDrop( Inventory.DropEventInfo e )
        {
            Destroy( itemUIs[e.Slot].gameObject );

            itemUIs.Remove( e.Slot );
        }
    }
}