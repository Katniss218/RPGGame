using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    public class ItemUI : MonoBehaviour
    {
        public PlayerInventory Inventory;
        public Vector2Int Slot;

        private Image icon;
        private TMPro.TextMeshProUGUI amountText;

        void Awake()
        {
            amountText = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            icon = this.GetComponentInChildren<Image>();
        }

        public void SetIcon( Sprite sprite )
        {
            icon.sprite = sprite;
        }

        public void SetAmount( int amount )
        {
            amountText.text = $"{amount}";
        }

        public void OnClickDrop()
        {
            if( Inventory != null )
            {
                Inventory.Drop( null, Slot );
            }
        }
    }
}