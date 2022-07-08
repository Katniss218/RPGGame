using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class HumanInventory : Inventory
    {
        public Item EquipHand;
        public Item EquipOffhand;

        public Item EquipHead;
        public Item EquipBody;
        public Item EquipLegs;
    }
}