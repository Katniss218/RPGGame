using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RPGGame.Serialization
{
    public static class SerializationManager
    {
        // some sort of dictionary of memberinfos, etc, for a given type? (so we don't have to read them every time).

        public static string serializedTemp = "";

        // Implementing this dictionary improves time performance when serializing approximately 100-fold (A HUNDRED!).
        // Deserializing seems to be around 3x more costly than serializing (tested on just 2 properties).
        private static Dictionary<Type, TypeSerializationInfo> typeDict = new Dictionary<Type, TypeSerializationInfo>();

        private static TypeSerializationInfo PopulateTypeDict(Type type )
        {
            PropertyInfo[] properties = type.GetProperties();

            List<TypeSerializationInfo.PropertyData> persistedProperties = new List<TypeSerializationInfo.PropertyData>();
            foreach( var prop in properties )
            {
                object[] attrs = prop.GetCustomAttributes( typeof( PersistAttribute ), true );

                if( attrs.Length > 0 )
                {
                    persistedProperties.Add( new TypeSerializationInfo.PropertyData((PersistAttribute)attrs[0], prop) );
                }
            }

            TypeSerializationInfo info = new TypeSerializationInfo() { PersistedProperties = persistedProperties };

            typeDict.Add( type, info );
            return info;
        }

        public static string SerializeObject( object obj )
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // writes serializable data to a string.
            // Returns that string.

            Type type = obj.GetType();

            TypeSerializationInfo info;
            if( !typeDict.TryGetValue( type, out info ) )
            {
                info = PopulateTypeDict( type );
            }

            List<string> props = new List<string>();
            foreach( var propData in info.PersistedProperties )
            {
                object value = propData.PropInfo.GetValue( obj );
                //props.Add( $"({prop.PropertyType.FullName}) {prop.Name} = {value.ToString()}" );
                props.Add( $"{propData.Attr.Name}={value.ToString()}" );
            }

            string s = string.Join( '\n', props );

            sw.Stop();
            UnityEngine.Debug.LogWarning( "ser: " + sw.ElapsedTicks );

            return s;
        }

        public static void PopulateObject( object obj, string data )
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Type type = obj.GetType();

            TypeSerializationInfo info;
            if( !typeDict.TryGetValue( type, out info ) )
            {
                info = PopulateTypeDict( type );
            }

            string[] props = data.Split( '\n' );

            foreach( var propData in info.PersistedProperties )
            {
                // if the type is a serializable, call deserialize on it.
                // if the type is primitive, read the string.

                string val = props.Where( s => s.StartsWith( $"{propData.Attr.Name}=" ) ).First().Split('=').Last();

                float val2 = float.Parse( val );
                propData.PropInfo.SetValue( obj, val2 );
            }

            sw.Stop();
            UnityEngine.Debug.LogWarning( "deser: " + sw.ElapsedTicks );
        }
    }
}