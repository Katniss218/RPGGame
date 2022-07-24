using RPGGame.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RPGGame.Serialization
{
    /// <summary>
    /// Manages the area switching, saving, and loading.
    /// </summary>
    public static class SaveAreaManager
    {
        public static string CurrentSave { get; private set; } = null;
        public static string CurrentArea { get; private set; } = null;

        /// <summary>
        /// Loads the game from a save named 'save', the area loaded depends on where the player is.
        /// </summary>
        public static void Load( string save )
        {
            Load( save, "default_area" );
        }

        private static void Load( string save, string area )
        {
            if( save != null )
            {
                Save( save );
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            CurrentSave = save;
            CurrentArea = area;

            SaveGameUtils.LoadArea( save, area );
            SaveGameUtils.LoadPlayer( save );

            sw.Stop();
            Debug.LogWarning( $"Loaded '{save}:{area}' in {sw.ElapsedTicks / 10000f} ms" );
        }

        /// <summary>
        /// Saves the game under the name 'save', the area saved depends on where the player is.
        /// </summary>
        public static void Save( string save )
        {
            Save( save, CurrentArea );
        }

        private static void Save( string save, string area )
        {
            if( area == null )
            {
                throw new InvalidOperationException( "Area can't be null." );
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            SaveGameUtils.SaveArea( save, area );
            SaveGameUtils.SavePlayer( save, PlayerManager.Player.gameObject );

            sw.Stop();
            Debug.LogWarning( $"Saved '{save}:{area}' in {sw.ElapsedTicks / 10000f} ms" );
        }

        public static void SwitchAreas( string newArea )
        {
            Save( CurrentSave, CurrentArea );

            Load( CurrentSave, newArea );
        }
    }
}