using RPGGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Assets.Providers
{

    public class ItemProviderResources : IAssetProvider<Item>
    {
        public const string ITEMS_PATH = "_ OBJECTS _/Items";

        public IEnumerable<(string assetID, Item obj)> GetAll()
        {
            List<(string assetID, Item obj)> allItems = new List<(string assetID, Item obj)>();

            Item[] items = Resources.LoadAll<Item>( ITEMS_PATH );
            foreach( var item in items )
            {
                allItems.Add( (item.ID, item) );
            }

            return allItems;
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