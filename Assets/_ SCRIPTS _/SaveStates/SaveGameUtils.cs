using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RPGGame.Editor;
using RPGGame.Player;
using RPGGame.Progression.Dialogues;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RPGGame.SaveStates
{
    /// <summary>
    /// Interfaces between the saves and actual files on disk.
    /// </summary>
    public static class SaveGameUtils
    {
        // multiple saves, each save contains every area.
        // when saving as, every area and player data and more is copied to the new save's directory.

        public const string DEFAULT_SAVE_STATE = "_init";

        /// <summary>
        /// Returns the saves root directory.
        /// </summary>
        public static string SavesDirectory => Path.Combine( Main.GameDirectory, "saves" );

        /// <summary>
        /// Returns the root directory for a particular save.
        /// </summary>
        public static string GetSaveDirectory( string save )
        {
            // saves/<save>

            if( save == null )
            {
                return Path.Combine( SavesDirectory, DEFAULT_SAVE_STATE );
            }
            return Path.Combine( SavesDirectory, save );
        }

        /// <summary>
        /// Returns the root directory for a particular area within a particular save.
        /// </summary>
        public static string GetAreaSaveDirectory( string save, string areaID )
        {
            // saves/<save>/areas/<area>

            return Path.Combine( GetSaveDirectory( save ), "areas", areaID );
        }

        static string GetAreaObjectsFile( string save, string areaID )
        {
            return Path.Combine( GetAreaSaveDirectory( save, areaID ), "objects.json" );
        }

        static string GetSaveDataFile( string save )
        {
            return Path.Combine( GetSaveDirectory( save ), "save.json" );
        }

        static string GetSaveObjectsFile( string save )
        {
            return Path.Combine( GetSaveDirectory( save ), "objects.json" );
        }

        static string GetSaveDialoguesFile( string save )
        {
            return Path.Combine( GetSaveDirectory( save ), "dialogues.json" );
        }

        static string GetSaveQuestsFile( string save )
        {
            return Path.Combine( GetSaveDirectory( save ), "quests.json" );
        }

        //
        //
        //

        public static void StartSavingLoading()
        {

        }

        public static void EndSavingLoading()
        {

        }

        /// <summary>
        /// Loads the empty scene and deserializes a particular save and area into it.
        /// </summary>
        public static void LoadArea( string save, string areaID )
        {
            string path = GetAreaObjectsFile( save, areaID );

            string str = File.ReadAllText( path );
            SerializationManager.LoadScene( JsonConvert.DeserializeObject<JObject>( str ) );
        }

        /// <summary>
        /// Saves the current scene as a particular save and area.
        /// </summary>
        public static void SaveArea( string save, string areaID )
        {
            string path = GetAreaObjectsFile( save, areaID );

            string str = JsonConvert.SerializeObject( SerializationManager.SaveScene(), Formatting.Indented );

            DirectoryEx.EnsureExists( Path.GetDirectoryName( path ) );
            File.WriteAllText( path, str );
        }
        
        /// <summary>
        /// Loads the empty scene and deserializes a particular save and area into it.
        /// </summary>
        public static void LoadDialogues( string save )
        {
            string path = GetSaveDialoguesFile( save );

            string str = File.ReadAllText( path );

            JToken json = JsonConvert.DeserializeObject<JToken>( str );
            foreach( var d in json )
            {
                DialogueManager.Register( (Dialogue)d );
            }
        }

        /// <summary>
        /// Saves the current scene as a particular save and area.
        /// </summary>
        public static void SaveDialogues( string save )
        {
            string path = GetSaveDialoguesFile( save );

            DialogueWrapper w = Object.FindObjectOfType<DialogueWrapper>();

            List<JToken> json = new List<JToken>();
            foreach( Dialogue d in w.Dialogues )
            {
                json.Add( d );
            }

            string str = JsonConvert.SerializeObject( json, Formatting.Indented );

            DirectoryEx.EnsureExists( Path.GetDirectoryName( path ) );
            File.WriteAllText( path, str );
        }
        
        /// <summary>
        /// Spawns the player from a particular save into the current scene.
        /// </summary>
        public static void LoadPlayer( string save )
        {
            string path = GetSaveObjectsFile( save );

            string str = File.ReadAllText( path );

            JObject json = JsonConvert.DeserializeObject<JObject>( str );

            RPGObject player = PlayerManager.SpawnPlayer();
            PlayerManager.SetUpPlayerHooks();
            SerializationHelper.SetDataGameObject( player, json );
        }

        /// <summary>
        /// Saves the current player into a particular save.
        /// </summary>
        public static void SavePlayer( string save )
        {
            SavePlayer( save, PlayerManager.Player );
        }

        /// <summary>
        /// Saves the specified player object into a particular save.
        /// </summary>
        /// <param name="player">The specific player object to serialize.</param>
        public static void SavePlayer( string save, RPGObject player )
        {
            string path = GetSaveObjectsFile( save );

            string str = JsonConvert.SerializeObject( SerializationHelper.GetDataRpgObject( player ), Formatting.Indented );

            DirectoryEx.EnsureExists( Path.GetDirectoryName( path ) );
            File.WriteAllText( path, str );
        }

        public static SaveData LoadSaveData( string save )
        {
            string path = GetSaveDataFile( save );

            string str = File.ReadAllText( path );

            SaveData data = JsonConvert.DeserializeObject<SaveData>( str );

            return data;
        }

        public static void SaveSaveData( string save, SaveData data )
        {
            string path = GetSaveDataFile( save );

            string str = JsonConvert.SerializeObject( data );

            DirectoryEx.EnsureExists( Path.GetDirectoryName( path ) );
            File.WriteAllText( path, str );
        }
    }

    //
    //      Save file structure
    //
    /*
            saves
                <save>
                    save.json           -- non-object scene-independent info about the save (display name, current area the player is in, quests, etc)
                    objects.json        -- contains persistent area-independent objects (player, player minions, etc etc)
                    areas
                        <area>
                            objects.json

    */

    //      save.json structure:
    //
    /*
    
    {
        CurrentArea (string)
    }
    

    */
}