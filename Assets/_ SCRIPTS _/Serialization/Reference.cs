using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Serialization
{
    /// <summary>
    /// A helper static class for serializing references to things.
    /// </summary>
    public static class Reference
    {
        /*  The bit in {} brackets is THE reference.
        
        "AssetRefField": 
        {
            "$ref": "$asset;<assetID>"
        }

        "ObjectRefField": 
        {
            "$ref": "$id;<guid>"
        }

        */

        /// <summary>
        /// Use this as a key when serializing references to JSON to ensure consistency.
        /// </summary>
        public const string KEY = "$ref";

        const char REF = '$';
        const char REF_SEPARATOR = ';';

        const string ID = "id";
        const string ASSET = "asset";

        //
        //      OBJECT REFERENCES
        //

        /// <summary>
        /// Returns a JSON reference to a given object.
        /// </summary>
        public static JToken ObjectRef( RPGObject obj )
        {
            return new JObject()
            {
                { KEY, ObjectToReference( obj ) }
            };
        }

        /// <summary>
        /// Returns an object from a given JSON reference.
        /// </summary>
        public static RPGObject ObjectRef( JToken jsonRef )
        {
            return ReferenceToObject( (string)jsonRef[KEY] );
        }

        static string ObjectToReference( RPGObject obj )
        {
            return $"{REF}{ASSET}{REF_SEPARATOR}{obj.guid}";
        }

        static RPGObject ReferenceToObject( string refStr )
        {
            string start = $"{REF}{ID}{REF_SEPARATOR}";

            if( !refStr.StartsWith( start ) )
            {
                throw new ArgumentException( $"A reference string must follow the format '{start}<guid>'." );
            }

            Guid guid = Guid.Parse( refStr.Substring( start.Length ) );

            return RPGObject.Get( guid );
        }

        //
        //      ASSET REFERENCES
        //

        /// <summary>
        /// Returns a JSON reference to a given assetID.
        /// </summary>
        public static JToken AssetRef( string assetID )
        {
            return new JObject()
            {
                { KEY, AssetIDToReference( assetID ) }
            };
        }

        /// <summary>
        /// Returns an assetID from a given JSON reference.
        /// </summary>
        public static string AssetRef( JToken jsonRef )
        {
            return ReferenceToAssetID( (string)jsonRef[KEY] );
        }

        static string AssetIDToReference( string assetID )
        {
            return $"{REF}{ASSET}{REF_SEPARATOR}{assetID}";
        }

        static string ReferenceToAssetID( string refStr )
        {
            string start = $"{REF}{ASSET}{REF_SEPARATOR}";

            if( !refStr.StartsWith( start ) )
            {
                throw new ArgumentException( $"A reference string must follow the format '{start}<assetID>'." );
            }

            return refStr.Substring( start.Length );
        }
    }
}