using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [RequireComponent( typeof( GridInventory ) )]
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