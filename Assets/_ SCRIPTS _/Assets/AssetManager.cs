using RPGGame.Assets.Providers;
using RPGGame.Items;
using RPGGame.Items.LootTables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame.Assets
{
    public static class AssetManager
    {
        public static AssetRegistry<Item> Items = new AssetRegistry<Item>( new ItemProviderResources() );
        public static AssetRegistry<LootTable> LootTables = new AssetRegistry<LootTable>( new LootTableProviderResources() );
        public static AssetRegistry<GameObject> Mobs = new AssetRegistry<GameObject>( new MobProviderResources() );

        public static AssetRegistry<GameObject> Prefabs = new AssetRegistry<GameObject>( new PrefabProviderResources() );
        public static AssetRegistry<AudioClip> AudioClips = new AssetRegistry<AudioClip>( new AudioClipProviderResources() );
        public static AssetRegistry<Sprite> Sprites = new AssetRegistry<Sprite>( new SpriteProviderResources() );
    }
}