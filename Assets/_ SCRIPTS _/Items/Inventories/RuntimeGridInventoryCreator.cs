using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items.Inventories
{
    /// <summary>
    /// Use this in the editor to generate the inventory slots for a serialized grid inventory.
    /// </summary>
    [RequireComponent( typeof( GridInventory ) )]
    [ExecuteInEditMode]
    public class RuntimeGridInventoryCreator : MonoBehaviour
    {
        public int sizeX;
        public int sizeY;

        void Awake()
        {
            GridInventory inv = this.GetComponent<GridInventory>();

            inv.SetSize( sizeX, sizeY );
        }
    }
}