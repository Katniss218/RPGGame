using RPGGame.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame
{
    public static class AssetManager
    {
        const string ITEMS_PATH = "_ OBJECTS _/Items";
        const string MOBS_PATH = "_ OBJECTS _/Mobs";

        private static Dictionary<string, Item> allItems = null;
        private static Dictionary<string, GameObject> allMobs = null;

        const string PREFABS_PATH = "Prefabs";
        const string MESHES_PATH = "Meshes";
        const string MATERIALS_PATH = "Materials";
        const string SOUNDS_PATH = "Sounds";
        const string SPRITES_PATH = "Sprites";

        private static Dictionary<string, GameObject> allPrefabs = new Dictionary<string, GameObject>();
        private static Dictionary<string, Mesh> allMeshes = new Dictionary<string, Mesh>();
        private static Dictionary<string, Material> allMaterials = new Dictionary<string, Material>();
        private static Dictionary<string, AudioClip> allAudioClips = new Dictionary<string, AudioClip>();
        private static Dictionary<string, Sprite> allSprites = new Dictionary<string, Sprite>();

        //
        //      ITEMS
        //

        private static void LoadItems()
        {
            allItems = new Dictionary<string, Item>();

            Item[] items = Resources.LoadAll<Item>( ITEMS_PATH );
            foreach( var item in items )
            {
                allItems.Add( item.ID, item );
            }
        }

        public static Item GetItem( string id )
        {
            if( allItems == null )
            {
                LoadItems();
            }

            if( allItems.TryGetValue( id, out Item item ) )
            {
                return item;
            }
            throw new InvalidOperationException( $"Couldn't get the item with an ID '{id}'. Item doesn't exist." );
        }

        /// <summary>
        /// Returns an array containing every single item available in the game.
        /// </summary>
        public static Item[] GetItems()
        {
            if( allItems == null )
            {
                LoadItems();
            }

            return allItems.Values.ToArray();
        }

        //
        //      MOBS
        //

        private static void LoadMobs()
        {
            allMobs = new Dictionary<string, GameObject>();

            GameObject[] mobs = Resources.LoadAll<GameObject>( MOBS_PATH );
            foreach( var mob in mobs )
            {
                allMobs.Add( mob.name, mob );
            }
        }

        public static GameObject GetMob( string id )
        {
            if( allMobs == null )
            {
                LoadMobs();
            }

            if( allMobs.TryGetValue( id, out GameObject mob ) )
            {
                return mob;
            }
            throw new InvalidOperationException( $"Couldn't get the mob with an ID '{id}'. Mob doesn't exist." );
        }

        /// <summary>
        /// Returns an array containing every single mob available in the game.
        /// </summary>
        public static GameObject[] GetMobs()
        {
            if( allMobs == null )
            {
                LoadMobs();
            }

            return allMobs.Values.ToArray();
        }

        //
        //      PREFABS
        //

        public static GameObject GetPrefab( string path )
        {
            if( allPrefabs.TryGetValue( path, out GameObject prefab ) )
            {
                return prefab;
            }

            if( !path.StartsWith( PREFABS_PATH ) )
            {
                throw new Exception( "Tried to look up a prefab in a wrong folder." );
            }

            prefab = Resources.Load<GameObject>( path );
            if( prefab == null )
            {
                throw new InvalidOperationException( $"Couldn't get the prefab with a path '{path}'. Prefab doesn't exist." );
            }
            allPrefabs.Add( path, prefab );
            return prefab;
        }
        //
        //      MESHES
        //

        public static Mesh GetMesh( string path )
        {
            if( allMeshes.TryGetValue( path, out Mesh mesh ) )
            {
                return mesh;
            }

            if( !path.StartsWith( MESHES_PATH ) )
            {
                throw new Exception( "Tried to look up a mesh in a wrong folder." );
            }

            mesh = Resources.Load<Mesh>( path );
            if( mesh == null )
            {
                throw new InvalidOperationException( $"Couldn't get the mesh with a path '{path}'. Mesh doesn't exist." );
            }
            allMeshes.Add( path, mesh );
            return mesh;
        }

        //
        //      MATERIALS
        //

        public static Material GetMaterial( string path )
        {
            if( allMaterials.TryGetValue( path, out Material material ) )
            {
                return material;
            }

            if( !path.StartsWith( MATERIALS_PATH ) )
            {
                throw new Exception( "Tried to look up a material in a wrong folder." );
            }

            material = Resources.Load<Material>( path );
            if( material == null )
            {
                throw new InvalidOperationException( $"Couldn't get the material with a path '{path}'. Material doesn't exist." );
            }
            allMaterials.Add( path, material );
            return material;
        }

        //
        //      SOUNDS / AUDIO CLIPS
        //

        public static AudioClip GetAudioClip( string path )
        {
            if( allAudioClips.TryGetValue( path, out AudioClip audioClip ) )
            {
                return audioClip;
            }

            if( !path.StartsWith( SOUNDS_PATH ) )
            {
                throw new Exception( "Tried to look up a audio clip in a wrong folder." );
            }

            audioClip = Resources.Load<AudioClip>( path );
            if( audioClip == null )
            {
                throw new InvalidOperationException( $"Couldn't get the audio clip with a path '{path}'. Audio clip doesn't exist." );
            }
            allAudioClips.Add( path, audioClip );
            return audioClip;
        }

        //
        //      SPRITES
        //

        public static Sprite GetSprite( string path )
        {
            if( allSprites.TryGetValue( path, out Sprite sprite ) )
            {
                return sprite;
            }

            if( !path.StartsWith( SPRITES_PATH ) )
            {
                throw new Exception( "Tried to look up a sprite in a wrong folder." );
            }

            sprite = Resources.Load<Sprite>( path );
            if( sprite == null )
            {
                throw new InvalidOperationException( $"Couldn't get the sprite clip with a path '{path}'. Sprite doesn't exist." );
            }
            allSprites.Add( path, sprite );
            return sprite;
        }
    }
}