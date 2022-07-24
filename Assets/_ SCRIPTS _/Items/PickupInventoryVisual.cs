using RPGGame.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [RequireComponent( typeof( PickupInventory ) )]
    public class PickupInventoryVisual : MonoBehaviour
    {
        private GameObject model = null;

        private PickupInventory inventory;

        void Awake()
        {
            inventory = this.GetComponent<PickupInventory>();
            inventory.onPickup?.AddListener( OnPickup );
            inventory.onDrop?.AddListener( OnDrop );
        }

        private void OnPickup( IInventory.PickupEventInfo e )
        {
            Destroy( model );
            model = null;

            List<(ItemStack, int orig)> items = e.Self.GetItemSlots();
            if( items.Count > 1 )
            {
                model = Instantiate( AssetManager.Prefabs.Get( "Prefabs/pickup_box" ), this.transform );
            }
            else
            {
                model = Instantiate( e.Item.model, this.transform );
            }
        }

        private void OnDrop( IInventory.DropEventInfo e )
        {
            //if( e.SlotOrigin == PlayerInventory.SLOT_HAND )
            // {
            Destroy( model );
            model = null;
            // }
        }
    }
}