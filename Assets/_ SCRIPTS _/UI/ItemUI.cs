using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class ItemUI : MonoBehaviour
    {
        public PlayerInventory Inventory;
        public Vector2Int Slot;

        public TMPro.TextMeshProUGUI Text { get; private set; }

        void Awake()
        {
            Text = this.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        }

        public void OnClickDrop()
        {
            Inventory.Drop( null, Slot );
        }
    }
}