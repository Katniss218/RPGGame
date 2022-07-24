using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Player;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace RPGGame.SaveStates
{
    /// <summary>
    /// Interfaces between the saves and actual files on disk.
    /// </summary>
    public static class SaveGameUtils
    {
        // multiple saves, each save contains every area.
        // when saving as, every area and player data and more is copied to the new save's directory.
        
        /// <summary>
        /// Returns the saves root directory.
        /// </summary>
        public static string SavesDirectory => Path.Combine( Main.GameDirectory, "saves" );

        /// <summary>
        /// Returns the root directory for a particular save.
        /// </summary>
        public static string GetSaveDirectory( string save )
        {
            if( save == null )
            {
                return Path.Combine( SavesDirectory, "_init" );
            }
            return Path.Combine( SavesDirectory, save );
        }

        /// <summary>
        /// Returns the root directory for a particular area within a particular save.
        /// </summary>
        public static string GetAreaSaveDirectory( string save, string areaID )
        {
            return Path.Combine( GetSaveDirectory( save ), areaID );
        }

        static string GetAreaObjectsSaveFile( string save, string areaID )
        {
            return Path.Combine( GetAreaSaveDirectory( save, areaID ), "objects.json" );
        }

        static string GetMetaSaveFile( string save )
        {
            return Path.Combine( GetSaveDirectory( save ), "meta.json" );
        }
        
        static string GetGlobalObjectsFile( string save )
        {
            return Path.Combine( GetSaveDirectory( save ), "objects.json" );
        }

        /// <summary>
        /// Loads the empty scene and deserializes a particular save and area into it.
        /// </summary>
        public static void LoadArea( string save, string areaID )
        {
            string objectsFilePath = GetAreaObjectsSaveFile( save, areaID );

            string savedText = File.ReadAllText( objectsFilePath );
            SerializationManager.LoadScene( JsonConvert.DeserializeObject<JObject>( savedText ) );
        }

        /// <summary>
        /// Saves the current scene as a particular save and area.
        /// </summary>
        public static void SaveArea( string save, string areaID )
        {
            string objectsFilePath = GetAreaObjectsSaveFile( save, areaID );

            string savedText = JsonConvert.SerializeObject( SerializationManager.SaveScene(), Formatting.Indented );

            DirectoryEx.EnsureExists( Path.GetDirectoryName( objectsFilePath ) );
            File.WriteAllText( objectsFilePath, savedText );
        }

        /// <summary>
        /// Spawns the player from a particular save into the current scene.
        /// </summary>
        public static void LoadPlayer( string save )
        {
            string globalObjectsFilePath = GetGlobalObjectsFile( save );

            string globalText = File.ReadAllText( globalObjectsFilePath );
            JObject globalJson = JsonConvert.DeserializeObject<JObject>( globalText );
            GameObject player = PlayerManager.SpawnPlayer();
            SerializationHelper.SetDataGameObject( player, globalJson );
        }

        /// <summary>
        /// Saves the current player into a particular save.
        /// </summary>
        public static void SavePlayer( string save )
        {
            SavePlayer( save, PlayerManager.Player.gameObject );
        }

        /// <summary>
        /// Saves the specified player object into a particular save.
        /// </summary>
        /// <param name="player">The specific player object to serialize.</param>
        public static void SavePlayer( string save, GameObject player )
        {
            string globalObjectsFilePath = GetGlobalObjectsFile( save );

            string globalText = JsonConvert.SerializeObject( SerializationHelper.GetDataGameObject( player ), Formatting.Indented );

            DirectoryEx.EnsureExists( Path.GetDirectoryName( globalObjectsFilePath ) );
            File.WriteAllText( globalObjectsFilePath, globalText );
        }
    }

    //
    //      Save file structure
    //
    /*
            saves
                <save>
                    save.json
                    objects.json
                    areas
                        <area>
                            objects.json

    */

    //      save.json structure:
    //
    /*
    
    {
        CurrentArea (string)
        Player (object)
    }
    

    */
}