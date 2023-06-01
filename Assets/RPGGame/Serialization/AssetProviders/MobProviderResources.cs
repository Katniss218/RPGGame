using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AssetManagement.Providers;

namespace RPGGame.Assets.Providers
{
    public class MobProviderResources : IAssetProvider<GameObject>
    {
        public const string PATH = "Prefabs/Mobs";

        public IEnumerable<(string assetID, GameObject obj)> GetAll()
        {
            List<(string assetID, GameObject obj)> all = new List<(string assetID, GameObject obj)>();

            GameObject[] loaded = Resources.LoadAll<GameObject>( PATH );
            foreach( var obj in loaded )
            {
                all.Add( (obj.name, obj) );
            }

            return all;
        }

        public bool Get( string assetID, out GameObject obj )
        {
            obj = default;
            return false;
        }

        public bool GetAssetID( GameObject obj, out string assetID )
        {
            assetID = default;
            return false;
        }
    }
}
