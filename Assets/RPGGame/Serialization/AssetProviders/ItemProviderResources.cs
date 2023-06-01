using RPGGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AssetManagement.Providers;

namespace RPGGame.Assets.Providers
{
    public class ItemProviderResources : IAssetProvider<Item>
    {
        public const string PATH = "_ OBJECTS _/Items";

        public IEnumerable<(string assetID, Item obj)> GetAll()
        {
            List<(string assetID, Item obj)> all = new List<(string assetID, Item obj)>();

            Item[] loaded = Resources.LoadAll<Item>( PATH );
            foreach( var obj in loaded )
            {
                all.Add( (obj.ID, obj) );
            }

            return all;
        }

        public bool Get( string assetID, out Item obj )
        {
            obj = default;
            return false;
        }

        public bool GetAssetID( Item obj, out string assetID )
        {
            assetID = default;
            return false;
        }
    }
}