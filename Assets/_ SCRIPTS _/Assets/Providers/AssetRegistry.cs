using RPGGame.Assets.Providers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Assets
{
    /// <summary>
    /// A self contained registry for one type of asset.
    /// </summary>
    public class AssetRegistry<T>
    {
        private IAssetProvider<T>[] providers;

        private IDictionary<string, T> registry = new Dictionary<string, T>();
        private IDictionary<T, string> reverseRegistry = new Dictionary<T, string>();

        private bool isLazyLoaded = false;

        /// <summary>
        /// Creates a registry with no asset providers. Use this if you want to only register assets manually.
        /// </summary>
        public AssetRegistry()
        {
            this.providers = new IAssetProvider<T>[] { };
        }

        /// <summary>
        /// Creates a registry with one asset provider
        /// </summary>
        public AssetRegistry( IAssetProvider<T> assetProvider )
        {
            this.providers = new IAssetProvider<T>[]
            {
                assetProvider
            };
        }

        /// <summary>
        /// Creates a registry with multiple asset providers.
        /// </summary>
        public AssetRegistry( params IAssetProvider<T>[] assetProviders )
        {
            this.providers = assetProviders;
        }

        private void TryLazyLoad()
        {
            // Already loaded and wasn't cleared - return.
            if( isLazyLoaded )
            {
                return;
            }

            // Ask every provider for their assets.
            foreach( var provider in providers )
            {
                IEnumerable<(string assetID, T obj)> objs = provider.GetAll();

                foreach( var (assetID, obj) in objs )
                {
                    registry.Add( assetID, obj );
                    reverseRegistry.Add( obj, assetID );
                }
            }

            isLazyLoaded = true;
        }

        private void TryLazyLoadOne( string assetID )
        {
            // Ask every provider for their asset.
            foreach( var provider in providers )
            {
                if( provider.Get( assetID, out T obj ) )
                {
                    registry.Add( assetID, obj );
                    reverseRegistry.Add( obj, assetID );
                }
            }
        }

        private void TryLazyLoadOne( T obj )
        {
            // Ask every provider for their asset.
            foreach( var provider in providers )
            {
                if( provider.GetAssetID( obj, out string assetID ) )
                {
                    registry.Add( assetID, obj );
                    reverseRegistry.Add( obj, assetID );
                }
            }
        }

        /// <summary>
        /// Clears the registry.
        /// </summary>
        public void ClearRegistry()
        {
            this.registry.Clear();
            this.reverseRegistry.Clear();
            isLazyLoaded = false;
        }

        /// <summary>
        /// Registers an asset with the specified assetID manually.
        /// </summary>
        public void Register( string assetID, T obj )
        {
            if( registry.ContainsKey( assetID ) )
            {
                throw new System.Exception( $"A '{typeof( T ).FullName}' asset with assetID '{assetID}' is already registered." );
            }

            registry.Add( assetID, obj );
            reverseRegistry.Add( obj, assetID );
        }

        /// <summary>
        /// Checks whether an asset with the specified assetID exists.
        /// </summary>
        public bool Exists( string assetID )
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            if( registry.ContainsKey( assetID ) )
            {
                return true;
            }

            // Sometimes assets can't be loaded all at once (because searching all paths is unavailable).
            TryLazyLoadOne( assetID );
            return registry.ContainsKey( assetID );
        }

        /// <summary>
        /// Returns an asset with the specified assetID. Throws an exception if none are found.
        /// </summary>
        public T Get( string assetID )
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            if( registry.TryGetValue( assetID, out T val ) )
            {
                return val;
            }

            // Sometimes assets can't be loaded all at once (because searching all paths is unavailable).
            TryLazyLoadOne( assetID );
            if( registry.TryGetValue( assetID, out val ) )
            {
                return val;
            }

            throw new System.Exception( $"A '{typeof( T ).FullName}' asset with assetID '{assetID}' is not registered." );
        }

        public IEnumerable<(string assetID, T obj)> GetAll()
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            List<(string assetID, T obj)> all = new List<(string assetID, T obj)>();

            foreach( var (key, obj) in registry )
            {
                all.Add( (key, obj) );
            }

            return all;
        }

        string GetAssetPath( T obj )
        {
            if( registry.Count == 0 )
            {
                TryLazyLoad();
            }

            if( reverseRegistry.TryGetValue( obj, out string assetID ) )
            {
                return assetID;
            }

            // Sometimes assets can't be loaded all at once (because searching all paths is unavailable).
            TryLazyLoadOne( obj );
            if( reverseRegistry.TryGetValue( obj, out assetID ) )
            {
                return assetID;
            }

            throw new System.Exception( $"A '{typeof( T ).FullName}' asset is not registered. Can't find an assetID." );
        }
    }
}