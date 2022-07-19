using Newtonsoft.Json.Linq;
using RPGGame.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_item_", menuName = "Items/Item", order = 1 )]
    public class Item : ScriptableObject, IIdentifyableAsset
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

        public static implicit operator JToken( Item self )
        {
#warning TODO - this won't work with derived classes. And they're not an easy problem. We'd need to keep the type info somewhere.
            return new JObject()
            {
#warning TODO - make getting ID path into a method on the asset manager class.
                { "$ref", $"$asset:item:{self.ID}" }
            };
        }

        public static explicit operator Item( JToken json )
        {
            return AssetManager.Items.Get( (string)json["$ref"] );
        }
    }
}