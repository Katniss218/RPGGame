using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_item_", menuName = "Items/Item", order = 1 )]
    public class Item : ScriptableObject
    {
        /// <summary>
        /// The identifier of the item.
        /// </summary>
        public string ID;

        public string DisplayName;
        public string Description;

        public Mesh mesh;
        public Material[] materials;

        public bool UseCustomCamera;
        public Vector3 CustomCameraRot;

        public int MaxStack = 1;
        public Vector2Int Size;

        /// <summary>
        /// Use this to compare if the items can be stacked together.
        /// </summary>
        public bool CanStackWith( Item other )
        {
            return this.ID == other.ID;
        }
    }
}