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

        /// <summary>
        /// If true, the loot table controller will drop its items on Start().
        /// </summary>
        public bool OnStart = false;

        /// <summary>
        /// True marks that this loot table controller has dropped its items and shouldn't drop them again.
        /// </summary>
        public bool Used = false;

        IInventory inventory = null;

        void Awake()
        {
            inventory = this.GetComponent<IInventory>();
        }

        void Start()
        {
            if( OnStart && !Used )
            {
                TryDropItems();
            }
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
                Debug.LogWarning( this.gameObject.name );
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
                { "OnStart", OnStart },
                { "Used", Used }
            };
        }

        public void SetData( JObject data )
        {
            this.LootTable = (LootTable)data["LootTable"];
            this.DropOnGround = (bool)data["DropOnGround"];
            this.OnStart = (bool)data["OnStart"];
            this.Used = (bool)data["Used"];
        }
    }
}