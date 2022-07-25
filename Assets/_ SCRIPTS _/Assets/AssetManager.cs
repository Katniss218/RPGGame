using RPGGame.Assets.Providers;
using RPGGame.Items;
using RPGGame.Items.LootTables;
using UnityEngine;

namespace RPGGame.Assets
{
    /// <summary>
    /// Manages the game assets that can be loaded at runtime.
    /// </summary>
    public static class AssetManager
    {
        /// <summary>
        /// Provides the items.
        /// </summary>
        public static AssetRegistry<Item> Items { get; private set; } = new AssetRegistry<Item>( new ItemProviderResources() );

        /// <summary>
        /// Provides the loot tables.
        /// </summary>
        public static AssetRegistry<LootTable> LootTables { get; private set; } = new AssetRegistry<LootTable>( new LootTableProviderResources() );

        /// <summary>
        /// Provides the mobs (prefabs).
        /// </summary>
        public static AssetRegistry<GameObject> Mobs { get; private set; } = new AssetRegistry<GameObject>( new MobProviderResources() );

        // ---

        /// <summary>
        /// Provides prefabs.
        /// </summary>
        public static AssetRegistry<GameObject> Prefabs { get; private set; } = new AssetRegistry<GameObject>( new PrefabProviderResources() );

        /// <summary>
        /// Provides audio clips.
        /// </summary>
        public static AssetRegistry<AudioClip> AudioClips { get; private set; } = new AssetRegistry<AudioClip>( new AudioClipProviderResources() );

        /// <summary>
        /// Provides sprites.
        /// </summary>
        public static AssetRegistry<Sprite> Sprites { get; private set; } = new AssetRegistry<Sprite>( new SpriteProviderResources() );
    }
}