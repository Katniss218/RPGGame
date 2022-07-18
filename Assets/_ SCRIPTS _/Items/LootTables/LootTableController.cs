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

        IInventory inventory = null;

        void Awake()
        {
            inventory = this.GetComponent<IInventory>();
        }

        /// <summary>
        /// Drops or adds the items from the loot table.
        /// </summary>
        public void DropItems()
        {
            List<ItemStack> items = LootTable.GetItems();

            if( DropOnGround )
            {
                foreach( var item in items )
                {
                    Main.CreatePickup( item.Item, item.Amount, this.transform.position, this.transform.rotation, false );
                }
            }
            else
            {
                Ensure.NotNull( LootTable );

                foreach( var item in items )
                {
                    inventory.TryAdd( item );
                }
            }
        }

        public JObject GetData()
        {
            return new JObject()
            {
                { "LootTable", LootTable },
                { "DropOnGround", DropOnGround }
            };
        }

        public void SetData( JObject data )
        {
            throw new System.NotImplementedException();
        }
    }
}