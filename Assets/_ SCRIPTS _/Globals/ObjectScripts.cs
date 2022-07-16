using RPGGame.Animation;
using RPGGame.Interactions;
using RPGGame.Items;
using RPGGame.UI;
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
        public void OnDeathDestroy( HealthHandler.DeathEventInfo e )
        {
            Destroy( e.Self.gameObject );
        }

        public void OnDeathDropInventory( HealthHandler.DeathEventInfo e )
        {
            IInventory inv = e.Self.GetComponent<IInventory>();

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
            IInventory inv = e.Self;
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
            if( e.Reason == IInventory.Reason.INVENTORY_REARRANGEMENT )
            {
                return;
            }

            CreatePickup( e.Item, e.Amount, e.Self.transform.position, e.Self.transform.rotation, e.Self is PlayerInventory );
        }

        public static void CreatePickup( Item item, int amount, Vector3 position, Quaternion rotation, bool applyForce )
        {

            const float HEIGHT_OFFSET = 0.25f;
            const float JITTER_RANGE = 0.05f;

            Vector3 offset = new Vector3( Random.Range( -JITTER_RANGE, JITTER_RANGE ), HEIGHT_OFFSET, Random.Range( -JITTER_RANGE, JITTER_RANGE ) );

            GameObject go = Instantiate( AssetManager.GetPrefab( "Prefabs/pickup" ), position + offset, Quaternion.identity );

            GameObject itemVisual = Instantiate( item.model, go.transform );

            PickupInventory inventory = go.GetComponent<PickupInventory>();
            inventory.SetCapacityAndPickUp( new ItemStack( item, amount ) );

            if( applyForce )
            {
                Rigidbody rigidbody = go.GetComponent<Rigidbody>();
                
                Vector3 dir = ((rotation * Vector3.forward) + new Vector3( 0.0f, 0.4f, 0.0f )).normalized;
                rigidbody.velocity = dir * 60f;
            }
        }

        /// <summary>
        /// Meant to run when you click on the "Open inventory" button.
        /// </summary>
        public void OnClick_Inventory( PlayerInventory inv )
        {
            if( UIWindow.ExistsAny<PlayerInventoryUI>() )
            {
                return;
            }
            PlayerInventoryUI.CreateUIWindow( inv );
        }

        /// <summary>
        /// Meant to run when you click on the "Open inventory" button.
        /// </summary>
        public void OnStartInteracting_Chest( Interactible.OnInteractEventInfo e )
        {
            UIWindow ui = GridInventoryUI.CreateUIWindow( e.Self.GetComponent<GridInventory>() );
            ui.rectTransform.anchoredPosition += new Vector2( 1000f, 0f );

            GenericAnimator anim = e.Self.GetComponentInChildren<GenericAnimator>();
            anim.PlayAnimation( 0.0f, 1.0f );

            ui.onClose.AddListener( () => e.Self.StopInteracting( e.Interactor ) );
        }

        /// <summary>
        /// Meant to run when you click on the "Open inventory" button.
        /// </summary>
        public void OnStopInteracting_Chest( Interactible.OnInteractEventInfo e )
        {
            GenericAnimator anim = e.Self.GetComponentInChildren<GenericAnimator>();
            anim.PauseAnimation();

            anim.PlayAnimation( Mathf.Clamp01( anim.PausedNormalizedTime ), -anim.PausedPreviousSpeed );
        }
    }
}