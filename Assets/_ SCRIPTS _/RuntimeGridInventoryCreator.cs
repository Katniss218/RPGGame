using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame
{
    [RequireComponent(typeof(GridInventory))]
    public class RuntimeGridInventoryCreator : MonoBehaviour
    {
        public int sizeX;
        public int sizeY;

        void Start()
        {
            GridInventory inv = this.GetComponent<GridInventory>();

            inv.SetSize( sizeX, sizeY );

            inv.TryAdd( new ItemStack( AssetManager.GetItem( "item.axe" ), 5 ) );
        }
    }
}