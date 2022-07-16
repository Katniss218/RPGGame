using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RPGGame.Serialization
{
    public class TypeSerializationInfo
    {
        public class PropertyData
        {
            public PersistAttribute Attr { get; set; }
            public PropertyInfo PropInfo { get; set; }

            public PropertyData( PersistAttribute attr, PropertyInfo propInfo )
            {
                this.Attr = attr;
                this.PropInfo = propInfo;
            }
        }

        public List<PropertyData> PersistedProperties = new List<PropertyData>();
#warning fields too.
#warning when deserializing a HealthHandler, the UI doesn't update immediately because the event doesn't fire.
    }
}