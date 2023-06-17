using Newtonsoft.Json.Linq;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlus.AssetManagement;

namespace RPGGame.Items.LootTables
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

        public string ID;

        public List<Entry> Items;

        /// <summary>
        /// Collapses the loot table and returns the resulting items.
        /// </summary>
        public List<ItemStack> GetItems()
        {
            List<ItemStack> items = new List<ItemStack>();

            foreach( var entry in Items )
            {
                if( UnityEngine.Random.Range( 0.0f, 1.0f ) > entry.DropChance )
                {
                    continue;
                }

                int count = UnityEngine.Random.Range( entry.MinAmount, entry.MaxAmount + 1 );

                ItemStack item = new ItemStack( entry.Item, count );
                items.Add( item );
            }

            return items;
        }

        //  ---------------------

        //      SERIALIZATION
        //

        // LootTable is serialized as the reference (doesn't contain a reference, it IS a reference).

        public static implicit operator JToken( LootTable self )
        {
            return Reference.AssetRef( self.ID );
        }

        public static explicit operator LootTable( JToken json )
        {
            return AssetRegistry.Get<LootTable>( Reference.AssetRef( json ) );
        }
    }
}