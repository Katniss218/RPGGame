using RPGGame.Audio;
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
    [RequireComponent( typeof( PlayerInventory ) )]
    [DisallowMultipleComponent]
    public class PlayerItemController : MonoBehaviour
    {
        private PlayerInventory inventory;

        private float lastUsedTimestamp;
        private float timeSinceLastUsed => Time.time - lastUsedTimestamp;

        void Awake()
        {
            inventory = this.GetComponent<PlayerInventory>();
        }

        void Update()
        {
            if( Input.GetMouseButtonDown( 0 ) )
            {
                if( !EventSystem.current.IsPointerOverGameObject() )
                {
                    UseEquipHand();
                }
            }
        }

        private Transform FindTarget()
        {
            // if weapon, target the closest to the cursor (or player, if cursor fails), but within range of the weapon.
            // if not weapon, target nothing.

            WeaponItem weaponHand = inventory.EquipHand.Item as WeaponItem;
            if( weaponHand == null )
            {
                return null;
            }

            Vector3 aoi = this.transform.position;

            Ray ray = Main.Camera.ScreenPointToRay( Input.mousePosition );
            if( Physics.Raycast( ray, out RaycastHit hitInfo, float.PositiveInfinity, 1 << 3 ) )
            {
                aoi = hitInfo.point;
            }

            Collider[] collidersInRange = Physics.OverlapSphere( this.transform.position, weaponHand.MeleeRange );

            (Transform t, float d) closestEnemy = (null, float.PositiveInfinity);

            foreach( var collider in collidersInRange )
            {
                if( collider.transform == this.transform )
                {
                    continue;
                }

                HealthHandler hitHealthScript = collider.GetComponent<HealthHandler>();
                if( hitHealthScript == null )
                {
                    continue;
                }

                float distance = Vector3.Distance( aoi, collider.transform.position );
                if( distance < closestEnemy.d )
                {
                    closestEnemy = (collider.transform, distance);
                }
            }

            return closestEnemy.t;
        }

        private void UseEquipHand()
        {
            UsableItem usableHand = inventory.EquipHand.Item as UsableItem;
            if( usableHand == null )
            {
                return;
            }

            if( timeSinceLastUsed < usableHand.UseTime )
            {
                return;
            }

            Transform target = FindTarget();

            usableHand.Use( this.transform, target );
            lastUsedTimestamp = Time.time;
        }
    }
}