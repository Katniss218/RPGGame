using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_loot_table_", menuName = "Items/Loot Table", order = -1 )]
    public class LootTable : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public Item Item;
            public int MinAmount = 1;
            public int MaxAmount = 1;
            public float DropChance = 1.0f;
        }

        public Entry[] items;
    }
}