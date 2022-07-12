using RPGGame.Audio;
using RPGGame.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Abilities
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(GridInventory))]
    public class ItemPickup : MonoBehaviour
    {
        public float PickupRange = 1.25f;

        public float PickupDelay = 0.1f;

        public int MaxItemStacksPerPickup = 1;

        float lastPickupTimestamp;
        float timeSinceLastPickup => Time.time - lastPickupTimestamp;

        GridInventory inventory;

        private void Awake()
        {
            inventory = GetComponent<GridInventory>();
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
        private void TryPickupNearby(float maxItemStacks)
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
                if( Time.time < pickupInv.createdTime + 1.5f )
                {
                    continue;
                }

                List<(ItemStack, int orig)> items = pickupInv.GetItemSlots();

                foreach( var (itemStack, orig) in items )
                {
                    int leftover = inventory.TryAdd( itemStack );

                    int amtPickedUp = itemStack.Amount - leftover;

                    if( amtPickedUp > 0 )
                    {
                        pickupInv.TryRemove( new ItemStack( itemStack.Item, amtPickedUp ) );
                        AudioManager.PlaySound( AssetManager.GetAudioClip( "Sounds/pickup" ), this.transform.position, 0.4f );

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