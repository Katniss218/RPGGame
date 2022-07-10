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

        public Image Icon { get; private set; }
        public TMPro.TextMeshProUGUI Text { get; private set; }

        void Awake()
        {
            Text = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            Icon = this.GetComponentInChildren<Image>();
        }

        public void SetIcon( Sprite sprite )
        {
            Icon.sprite = sprite;
        }

        public void OnClickDrop()
        {
            Inventory.Drop( null, Slot );
        }
    }
}