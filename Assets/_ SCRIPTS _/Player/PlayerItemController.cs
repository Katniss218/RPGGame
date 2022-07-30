using RPGGame.Items;
using RPGGame.Items.Inventories;
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

        private float lastUsedHandTimestamp;
        private float timeSinceLastUsedHand => Time.time - lastUsedHandTimestamp;
        
        private float lastUsedOffhandTimestamp;
        private float timeSinceLastUsedOffhand => Time.time - lastUsedOffhandTimestamp;

        void Awake()
        {
#warning TODO - prevent player from using offhand when interacting.
            inventory = this.GetComponent<PlayerInventory>();
        }

        void Update()
        {
            if( Input.GetMouseButtonDown( (int)MouseCode.LeftMouseButton ) )
            {
                if( !EventSystem.current.IsPointerOverGameObject() )
                {
                    UseHand();
                }
            }

            if( Input.GetMouseButtonDown( (int)MouseCode.RightMouseButton ) )
            {
                if( !EventSystem.current.IsPointerOverGameObject() )
                {
                    UseOffhand();
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

            Ray ray = CameraController.Camera.ScreenPointToRay( Input.mousePosition );
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

        private void UseHand()
        {
            UsableItem usableHand = inventory.EquipHand.Item as UsableItem;
            if( usableHand == null )
            {
                return;
            }

            if( timeSinceLastUsedHand < usableHand.UseTime )
            {
                return;
            }

            Transform target = FindTarget();

            usableHand.Use( this.transform, target );
            lastUsedHandTimestamp = Time.time;
        }

        private void UseOffhand()
        {
            UsableItem usableHand = inventory.EquipOffhand.Item as UsableItem;
            if( usableHand == null )
            {
                return;
            }

            if( timeSinceLastUsedOffhand < usableHand.UseTime )
            {
                return;
            }

            Transform target = FindTarget();

            usableHand.Use( this.transform, target );
            lastUsedOffhandTimestamp = Time.time;
        }
    }
}