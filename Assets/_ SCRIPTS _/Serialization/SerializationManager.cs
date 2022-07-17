using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        public static string serializedTemp = "";

        /// <summary>
        /// Returns the JSON containing the saved data of this particular object.
        /// </summary>
        public static JObject GetDataAll( GameObject obj )
        {
            JObject transformData = obj.transform.GetData();

            JToken componentData = GetComponentData( obj );

            JObject full = new JObject()
            {
                { "Transform", transformData },
                { "Components", componentData },
            };

            return full;
        }

        /// <summary>
        /// Applies the saved data to this object.
        /// </summary>
        /// <param name="data">The data. It's supposed to have come from the same object, saved earlier.</param>
        public static void SetDataAll( GameObject obj, JObject data )
        {
            JObject transformData = (JObject)data["Transform"];

            JToken componentData = data["Components"];

            obj.transform.SetData( transformData );
            SetComponentData( obj, componentData );
        }

        public static JToken GetComponentData( GameObject obj )
        {
            ISerializedBy[] serializedComponents = obj.GetComponents<ISerializedBy>();

            var componentsGroupedByType = serializedComponents.GroupBy( c => c.GetType() );

            JObject allComps = new JObject();

            // Save the components grouped by their type, and for each type, preserve their ordering (order within the type will matter, but each type can be rearranged).
            foreach( var group in componentsGroupedByType )
            {
                JArray compData = new JArray();

                foreach( var sc in group )
                {
                    JObject data = sc.GetData();
                    compData.Add( data );
                }

                allComps.Add( group.Key.FullName, compData );
            }

            return allComps;
        }

        public static void SetComponentData( GameObject obj, JToken data )
        {
            ISerializedBy[] serializedComponents = obj.GetComponents<ISerializedBy>();

            var componentsGroupedByType = serializedComponents.GroupBy( c => c.GetType() );

            foreach( var group in componentsGroupedByType )
            {
                string componentType = group.Key.FullName;
                JArray compData = (JArray)data[componentType];

                int i = 0;
                foreach( var sc in group )
                {
                    JObject d = (JObject)data[i];
                    sc.SetData( d );
                    i++;
                }
            }
        }
    }
}