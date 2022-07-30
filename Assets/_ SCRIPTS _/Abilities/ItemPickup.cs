using RPGGame.Audio;
using RPGGame.Items;
using RPGGame.Items.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent( typeof( PlayerInventory ) )]
    public class ItemPickup : MonoBehaviour
    {
        public float PickupRange = 1.25f;

        public float PickupDelay = 0.1f;

        public int MaxItemStacksPerPickup = 1;

        float lastPickupTimestamp;
        float timeSinceLastPickup => Time.time - lastPickupTimestamp;

        PlayerInventory inventory;

        private void Awake()
        {
            inventory = GetComponent<PlayerInventory>();
        }

        void Update()
        {
            if( timeSinceLastPickup >= PickupDelay )
            {
                TryPickupNearby( MaxItemStacksPerPickup );
                lastPickupTimestamp = Time.time;
            }
        }

        /// <summary>
        /// Try to pickup every item that's on the ground and is in range.
        /// </summary>
        private void TryPickupNearby( float maxItemStacks )
        {
            Collider[] collidersInRange = Physics.OverlapSphere( this.transform.position, PickupRange );

            float itemStacksLeftTopickUp = maxItemStacks;

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
                // Only pick from items that have been on the ground for some time.
                if( Time.time < pickupInv.CreatedTime + 1.5f )
                {
                    continue;
                }

                foreach( var (itemStack, orig) in pickupInv.GetItemSlots() )
                {
                    int leftover = inventory.TryAdd( itemStack );

                    int amtPickedUp = itemStack.Amount - leftover;

                    if( amtPickedUp > 0 )
                    {
                        AudioManager.PlaySound( itemStack.Item.PickupSound );
                        pickupInv.TryRemove( new ItemStack( itemStack.Item, amtPickedUp ) );

                        // Only count if we actually picked something up because \/
                        // Do not skip any inventories or itemstacks due to some items might only fall into some slots (mostly small items when your inventory is almost full).
                        itemStacksLeftTopickUp--;
                        if( itemStacksLeftTopickUp <= 0 )
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}