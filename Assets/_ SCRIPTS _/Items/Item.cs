using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_item_", menuName = "Items/Item", order = 1 )]
    public class Item : ScriptableObject
    {
        // Items are created from Item Prototypes. They can then be changed freely.

        /// <summary>
        /// IF of the prototype of the item.
        /// </summary>
        public string ID;

        public string DisplayName;
        public string Description;

        public int MaxStack = 1;
        public Vector2Int Size;
    }
}