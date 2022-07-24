using Newtonsoft.Json.Linq;
using RPGGame.Player;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace RPGGame.SaveStates
{
    /// <summary>
    /// Manages the currently loaded area, area switching, saving, loading, etc.
    /// </summary>
    public static class SaveStateManager
    {
        /// <summary>
        /// Returns the name/id of the currently loaded save state.
        /// </summary>
        public static string CurrentSave { get; private set; } = null;

        /// <summary>
        /// Returns the areaID of the currently loaded area.
        /// </summary>
        public static string CurrentAreaID { get; private set; } = null;

        public static void SwitchAreas( string newArea )
        {
            if( CurrentSave == null )
            {
                throw new InvalidOperationException( "Current save state can't be null." );
            }
            if( CurrentAreaID == null )
            {
                throw new InvalidOperationException( "Current area can't be null." );
            }

            Save( CurrentSave, CurrentAreaID );

            Load( CurrentSave, newArea );
        }

        /// <summary>
        /// Loads a save state from a file.
        /// </summary>
        /// <param name="save">The name of the save state (must exist).</param>
        public static void Load( string save )
        {
            SaveData data = SaveGameUtils.LoadSaveData( save );

            Load( save, data.CurrentArea );
        }

        private static void Load( string save, string area )
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            CurrentSave = save;
            CurrentAreaID = area;

            SaveGameUtils.LoadArea( save, area );
            SaveGameUtils.LoadPlayer( save );

            sw.Stop();
            Debug.LogWarning( $"Loaded '{save}:{area}' in {sw.ElapsedTicks / 10000f} ms" );
        }

        /// <summary>
        /// Saves the game to a save state file.
        /// </summary>
        /// <param name="save">The name of the save state. Overwritten if exists.</param>
        public static void Save( string save )
        {
            if( CurrentAreaID == null )
            {
                throw new InvalidOperationException( "Area can't be null." );
            }

            Save( save, CurrentAreaID );
        }

        private static void Save( string save, string area )
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            SaveGameUtils.SaveSaveData( save, new SaveData()
            {
                CurrentArea = CurrentAreaID
            } );
            SaveGameUtils.SaveArea( save, area );
            SaveGameUtils.SavePlayer( save, PlayerManager.Player.gameObject );

            sw.Stop();
            Debug.LogWarning( $"Saved '{save}:{area}' in {sw.ElapsedTicks / 10000f} ms" );
        }
    }
}