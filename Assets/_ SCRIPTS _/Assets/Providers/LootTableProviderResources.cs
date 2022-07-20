using RPGGame.Items.LootTables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Assets.Providers { 
    public class LootTableProviderResources : IAssetProvider<LootTable>
    {
        public const string PATH = "_ OBJECTS _/LootTables";

        public IEnumerable<(string assetID, LootTable obj)> GetAll()
        {
            List<(string assetID, LootTable obj)> all = new List<(string assetID, LootTable obj)>();

            LootTable[] loaded = Resources.LoadAll<LootTable>( PATH );
            foreach( var obj in loaded )
            {
                all.Add( (obj.ID, obj) );
            }

            return all;
        }

        public bool Get( string assetID, out LootTable obj )
        {
            obj = default;
            return false;
        }

        public bool GetAssetID( LootTable obj, out string assetID )
        {
            assetID = default;
            return false;
        }
    }
}
