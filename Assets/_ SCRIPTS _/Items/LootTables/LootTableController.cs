using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Globals;
using RPGGame.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items.LootTables
{
    public class LootTableController : MonoBehaviour, ISerializedComponent
    {
        public LootTable LootTable;

        /// <summary>
        /// If true, the items will be dropped onto the ground instead of into an inventory (in this mode it doesn't require an inventory to be present on the object).
        /// </summary>
        public bool DropOnGround = true;

        public bool Used = false;

        IInventory inventory = null;

        void Awake()
        {
            inventory = this.GetComponent<IInventory>();
        }

        /// <summary>
        /// Drops or adds the items from the loot table.
        /// </summary>
        public void TryDropItems()
        {
            if( Used )
            {
                return;
            }

#warning TODO - IMPORTANT! there seems to be some sort of call ordering issue here, resulting in exceptions when adding items by the loot table.
            List<ItemStack> items = LootTable.GetItems();

            Ensure.NotNull( LootTable );

            if( DropOnGround )
            {
                foreach( var item in items )
                {
                    Main.CreatePickup( item.Item, item.Amount, this.transform.position, this.transform.rotation, false );
                }
            }
            else
            {
                foreach( var item in items )
                {
                    inventory.TryAdd( item );
                }
            }

            Used = true;
        }

        public JObject GetData()
        {
            return new JObject()
            {
                { "LootTable", LootTable },
                { "DropOnGround", DropOnGround },
                { "Used", Used }
            };
        }

        public void SetData( JObject data )
        {
            this.LootTable = (LootTable)data["LootTable"];
            this.DropOnGround = (bool)data["DropOnGround"];
            this.Used = (bool)data["Used"];
        }
    }
}