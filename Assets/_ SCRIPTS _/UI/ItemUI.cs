using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.UI
{
    public class ItemUI : MonoBehaviour
    {
        public HumanInventory Inventory;
        public int Slot;

        public void OnClickDrop()
        {
            Inventory.Drop( Slot );
        }
    }
}