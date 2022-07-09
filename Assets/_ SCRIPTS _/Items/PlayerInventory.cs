using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class PlayerInventory : Inventory
    {
        public Item EquipHand;
        public Item EquipOffhand;

        public Item EquipHead;
        public Item EquipBody;
        public Item EquipLegs;

        private void Awake()
        {
            inventorySlots = new ItemSlot[6, 12];
            MakeEmptyWithNoBlocking();

            inventorySlots[0, 0] = ItemSlot.BlockingSlot();
            inventorySlots[5, 0] = ItemSlot.BlockingSlot();

            inventorySlots[0, 11] = ItemSlot.BlockingSlot();
            inventorySlots[5, 11] = ItemSlot.BlockingSlot();
        }
    }
}