using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGGame.Player
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [RequireComponent(typeof(HumanInventory))]
    public class PlayerItemController : MonoBehaviour
    {
        [SerializeField] private HumanInventory Inventory;
        [SerializeField] private float pickupRange;

        private float lastUsedTimestamp;

        void Update()
        {
            if( Input.GetMouseButtonDown( 0 ) )
            {
                if( !EventSystem.current.IsPointerOverGameObject() )
                {
                    UseEquipHand();
                }
            }
            if( Input.GetKeyDown( KeyCode.X ) )
            {
                TryPickupNearby();
            }
        }

        private void UseEquipHand()
        {
            UsableItem usableEquipHand = Inventory.EquipHand as UsableItem;
            if( usableEquipHand == null )
            {
                return;
            }

            if( Time.time >= lastUsedTimestamp + usableEquipHand.UseTime )
            {
                usableEquipHand.Use( this.transform );
                lastUsedTimestamp = Time.time;
            }
        }

        /// <summary>
        /// Try to pickup every item that's on the ground and is in range.
        /// </summary>
        private void TryPickupNearby()
        {
            Collider[] collidersInRange = Physics.OverlapSphere( this.transform.position, pickupRange );

            foreach( var collider in collidersInRange )
            {
                if( collider.transform == this.transform )
                {
                    continue;
                }

                PickupInventory pickupInv = collider.GetComponent<PickupInventory>();
                if( pickupInv == null )
                {
                    continue;
                }

                Inventory.PickUp( pickupInv );
                // Do not skip any inventories due to some items might only fall into some slots (like weapons, etc).
            }
        }
    }
}