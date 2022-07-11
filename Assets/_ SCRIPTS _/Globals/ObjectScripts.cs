using RPGGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RPGGame.Globals
{
    /// <summary>
    /// An editor class used to hold scripts that can then be used as inputs for events for various item types.
    /// </summary>
    [CreateAssetMenu( fileName = "_object_script_", menuName = "Objects/_SCRIPTS", order = 0 )]
    public class ObjectScripts : ScriptableObject
    {
        [SerializeField] private GameObject _pickupPrefab;

        public void OnDeathDestroy( HealthHandler.DeathEventInfo e )
        {
            Destroy( e.Self.gameObject );
        }

        public void OnDeathDropInventory( HealthHandler.DeathEventInfo e )
        {
            GridInventory inv = e.Self.GetComponent<GridInventory>();

            inv.Clear();
        }

        public void OnDeathRespawnPlayer( HealthHandler.DeathEventInfo e )
        {
            e.Self.transform.position = Main.PlayerRespawnPoint.transform.position;
            e.Self.transform.rotation = Main.PlayerRespawnPoint.transform.rotation;

            e.Self.SetHealth( e.Self.MaxHealth * 0.5f );
        }
        
        public void OnDropDestroyIfNone( IInventory.DropEventInfo e )
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

        public void OnDropCreatePickup( IInventory.DropEventInfo e )
        {
            const float HEIGHT_OFFSET = 0.25f;
            const float JITTER_RANGE = 0.05f;

            Vector3 offset = new Vector3( Random.Range( -JITTER_RANGE, JITTER_RANGE ), HEIGHT_OFFSET, Random.Range( -JITTER_RANGE, JITTER_RANGE ) );

            GameObject go = Instantiate( _pickupPrefab, e.Self.transform.position + offset, Quaternion.identity );

            MeshFilter meshFilter = go.GetComponentInChildren<MeshFilter>();
            meshFilter.mesh = e.Item.mesh;

            MeshRenderer meshRenderer = go.GetComponentInChildren<MeshRenderer>();
            meshRenderer.materials = e.Item.materials;

            PickupInventory inventory = go.GetComponent<PickupInventory>();
            inventory.SetCapacityAndPickUp( e.Item, e.Amount );

            if( e.Self is PlayerInventory )
            {
                Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                Vector3 dir = (e.Self.transform.forward + new Vector3( 0.0f, 0.4f, 0.0f )).normalized;
                rigidbody.velocity = dir * 60f;
            }
        }
    }
}