using RPGGame.Items;
using RPGGame.Items.Inventories;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RPGGame.Mobs
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    [RequireComponent( typeof( MobInventory ) )]
    [RequireComponent( typeof( MobTargetTracking ) )]
    [DisallowMultipleComponent]
    public class MobItemController : MonoBehaviour
    {
        private MobInventory inventory;
        private MobTargetTracking targetTracker;

        private float lastUsedTimestamp;
        private float timeSinceLastUsed => Time.time - lastUsedTimestamp;

        void Awake()
        {
            inventory = this.GetComponent<MobInventory>();
            targetTracker = this.GetComponent<MobTargetTracking>();
        }

        void Update()
        {
            UseEquipHand();
        }

        private void UseEquipHand()
        {
            if( targetTracker.Target == null )
            {
                return;
            }

            UsableItem usableHand = inventory.EquipHand as UsableItem;
            if( usableHand == null )
            {
                return;
            }

            if( timeSinceLastUsed < usableHand.UseTime )
            {
                return;
            }

            // Only use melee weapons when the mob can hit its target.
            WeaponItem weaponHand = inventory.EquipHand as WeaponItem;
            if( weaponHand != null && weaponHand.MeleeDamage > 0 )
            {
                if( Vector3.Distance( this.transform.position, targetTracker.Target.position ) > weaponHand.MeleeRange )
                {
                    return;
                }
            }

            usableHand.Use( this.transform, targetTracker.Target );
            lastUsedTimestamp = Time.time;
        }
    }
}