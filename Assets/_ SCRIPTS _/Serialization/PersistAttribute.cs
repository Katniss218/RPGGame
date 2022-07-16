using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Serialization
{
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public class PersistAttribute : Attribute
    {
        public string Name { get; set; }

        public PersistAttribute( string name )
        {
            this.Name = name;
        }
    }
}
