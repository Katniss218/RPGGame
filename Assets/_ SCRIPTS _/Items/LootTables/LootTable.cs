using Newtonsoft.Json.Linq;
using RPGGame.Assets;
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

        public static implicit operator JToken( LootTable self )
        {
            return new JObject()
            {
                { "$ref", SerializationHelper.ToReferenceString( ReferenceType.ASSET, self.ID ) }
            };
        }

        public static explicit operator LootTable( JToken json )
        {
            return AssetManager.LootTables.Get( SerializationHelper.ToAssetID( (string)json["$ref"] ).assetID );
        }
    }
}