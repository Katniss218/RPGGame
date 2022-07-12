using RPGGame.Items;
using RPGGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    [DisallowMultipleComponent]
    public class ItemDragAndDrop : MonoBehaviour
    {
        public static ItemStack cursorItem = ItemStack.Empty;

        static InventoryItemUI __instance = null;
        public static InventoryItemUI Instance
        {
            get
            {
                if( __instance == null ) __instance = FindObjectOfType<ItemDragAndDrop>().GetComponent<InventoryItemUI>();
                return __instance;
            }
        }

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = (RectTransform)this.transform;
            // We start without an item in hand.
            Instance.SetIcon( null );
            Instance.SetAmount( null );
        }

        void Update()
        {
            rectTransform.anchoredPosition = new Vector2( Input.mousePosition.x, Input.mousePosition.y );
        }
    }
}