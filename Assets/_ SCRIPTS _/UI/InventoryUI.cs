using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private HumanInventory _playerInv;
        [SerializeField] private RectTransform _uiContainer;

        [SerializeField] private GameObject _itemUIPrefab;

        Dictionary<int, RectTransform> _itemUIs = new Dictionary<int, RectTransform>();
        
        public void OnPickup( Inventory.PickupEventInfo e )
        {
            GameObject obj = Instantiate( _itemUIPrefab, _uiContainer );
            ItemUI itemUI = obj.GetComponent<ItemUI>();
            itemUI.Inventory = _playerInv;
            itemUI.Slot = e.Slot;

            _itemUIs.Add( e.Slot, (RectTransform)obj.transform );
        }

        public void OnDrop( Inventory.DropEventInfo e )
        {
            Destroy( _itemUIs[e.Slot].gameObject );

            _itemUIs.Remove( e.Slot );
        }
    }
}