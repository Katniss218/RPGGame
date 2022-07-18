using Newtonsoft.Json.Linq;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

            //  ---------------------

            //      SERIALIZATION
            //

            public static implicit operator JToken( Entry self )
            {
                return new JObject()
                {
                    { "Item", self.Item },
                    { "MinAmount", self.MinAmount },
                    { "MaxAmount", self.MaxAmount },
                    { "DropChance", self.DropChance },
                };
            }

            public static explicit operator Entry( JToken json )
            {
                return new Entry()
                {
                    Item = (Item)json["Item"],
                    MinAmount = (int)json["MinAmount"],
                    MaxAmount = (int)json["MaxAmount"],
                    DropChance = (float)json["DropChance"]
                };
            }
        }

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

        public static implicit operator JToken( LootTable self )
        {
            JArray jItems = new JArray();
            foreach( var item in self.Items )
            {
                jItems.Add( item );
            }

            return new JObject()
            {
                { "Items", jItems }
            };
        }

        public static explicit operator LootTable( JToken json )
        {
            LootTable self = new LootTable();

            self.Items = new List<Entry>();
            foreach( var entry in json["Items"] )
            {
                self.Items.Add( (Entry)entry );
            }

            return self;
        }
    }
}