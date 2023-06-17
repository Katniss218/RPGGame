using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityPlus.AssetManagement;

namespace RPGGame.Items.Inventories
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

            List<(ItemStack, int orig)> items = e.Self.GetItemSlots().ToList();
            if( items.Count > 1 )
            {
                model = Instantiate( AssetRegistry.Get<GameObject>( "builtin::Resources/Prefabs/pickup_box" ), this.transform );
            }
            else
            {
                model = Instantiate( e.Item.model, this.transform );
            }
        }

        private void OnDrop( IInventory.DropEventInfo e )
        {
            // do nothing. You can't add items to a drop. Once a drop is partially emptied, it should remain a box. When it's fully empty, it's gonna disappear anyway.
        }
    }
}