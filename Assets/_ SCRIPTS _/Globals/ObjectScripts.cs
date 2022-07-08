using RPGGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPGGame.Globals
{
    /// <summary>
    /// An editor class used to hold scripts that can then be used as inputs for events for various item types.
    /// </summary>
    [CreateAssetMenu( fileName = "_object_script_", menuName = "Objects/_SCRIPTS", order = 0 )]
    public class ObjectScripts : ScriptableObject
    {
        [SerializeField] private GameObject _pickupPrefab;

        public void OnDeathHandler( HealthHandler.OnDeathEventInfo e )
        {
            Destroy( e.Self.gameObject );
        }

        public void OnDropDestroyIfNone( Inventory.DropEventInfo e )
        {
            PickupInventory inv = e.Self as PickupInventory;
            if( inv == null )
            {
                throw new InvalidOperationException( $"Event Handler was attached to an invalid object'." );
            }

            if( inv.IsEmpty() )
            {
                Destroy( e.Self.gameObject );
            }
        }

        public void OnDropCreatePickup( Inventory.DropEventInfo e )
        {
            GameObject go = Instantiate( _pickupPrefab, e.Self.transform.position, Quaternion.identity );
            PickupInventory inventory = go.GetComponent<PickupInventory>();
            inventory.SetMaxCapacity( 1 );
            inventory.PickUp( e.Item );
        }
    }
}