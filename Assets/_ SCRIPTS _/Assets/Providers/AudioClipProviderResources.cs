using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Assets.Providers
{
    public class AudioClipProviderResources : IAssetProvider<AudioClip>
    {
        public IEnumerable<(string assetID, AudioClip obj)> GetAll()
        {
            return new (string, AudioClip)[] { };
        }

        public bool Get( string assetID, out AudioClip obj )
        {
            obj = Resources.Load<AudioClip>( assetID );
            return obj != null;
        }

        public bool GetAssetID( AudioClip obj, out string assetID )
        {
            assetID = default;
            return false;
        }
    }
}
