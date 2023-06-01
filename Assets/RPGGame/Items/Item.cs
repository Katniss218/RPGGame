using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using RPGGame.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AssetManagement;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_item_", menuName = "Items/Item", order = 1 )]
    public class Item : ScriptableObject
    {
        /// <summary>
        /// The identifier of the item.
        /// </summary>
        [field: SerializeField]
        public string ID { get; set; }

        public string DisplayName;
        public string Description;

        public GameObject model;

        public bool UseCustomCamera;
        public Vector3 CustomCameraRot;

        public int MaxStack = 1;
        public Vector2Int Size;

        public AudioClip PickupSound;
        public AudioClip DropSound;

        /// <summary>
        /// Use this to compare if the items can be stacked together.
        /// </summary>
        public bool CanStackWith( Item other )
        {
            return this.ID == other.ID;
        }

        //  ---------------------

        //      SERIALIZATION
        //

        // Item is serialized as the reference (doesn't contain a reference, it IS a reference).

        public static implicit operator JToken( Item self )
        {
            return Reference.AssetRef( self.ID );
        }

        public static explicit operator Item( JToken json )
        {
            return AssetRegistry<Item>.GetAsset( Reference.AssetRef(json) );
        }
    }
}