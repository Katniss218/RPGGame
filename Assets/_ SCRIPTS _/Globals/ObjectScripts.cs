using RPGGame.Animation;
using RPGGame.Interactions;
using RPGGame.Items;
using RPGGame.Items.Inventories;
using RPGGame.UI;
using RPGGame.UI.Windows;
using System;
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
        public void OnDeathDestroy( HealthHandler.DeathEventInfo e )
        {
            RPGObject.Destroy( e.Self.GetComponent<RPGObject>() );
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
                RPGObject.Destroy( e.Self.GetComponent<RPGObject>() );
            }
        }

        public void OnDropCreatePickup( IInventory.DropEventInfo e )
        {
            if( e.Reason == IInventory.Reason.INVENTORY_REARRANGEMENT )
            {
                return;
            }

            Main.CreatePickup( e.Item, e.Amount, e.Self.transform.position, e.Self.transform.rotation, e.Self is PlayerInventory );
        }

        /// <summary>
        /// Meant to run when you click on the "Open inventory" button.
        /// </summary>
        public void OnStartInteracting_Chest( Interactible.OnInteractEventInfo e )
        {
            UIWindow ui = GridInventoryUI.CreateUIWindow( e.Self.GetComponent<GridInventory>(), e.Self.transform );
            ui.rectTransform.anchoredPosition += new Vector2( 1000f, 0f );

            GenericAnimator anim = e.Self.GetComponentInChildren<GenericAnimator>();
            anim.PlayAnimation( 0.0f, 1.0f );

            ui.onClosed.AddListener( () =>
            {
                e.Self.StopInteracting( e.Interactor );
            } );
        }

        /// <summary>
        /// Meant to run when you click on the "Open inventory" button.
        /// </summary>
        public void OnStopInteracting_Chest( Interactible.OnInteractEventInfo e )
        {
            GenericAnimator anim = e.Self.GetComponentInChildren<GenericAnimator>();
            anim.PauseAnimation();

            anim.PlayAnimation( Mathf.Clamp01( anim.PausedNormalizedTime ), -anim.PausedPreviousSpeed );
            List<UIWindow> windows = UIWindow.GetFor( e.Self.transform, true );
            foreach( var window in windows )
            {
#warning TODO - what if the window pop up is NOT a result of an interaction? Right now it closes all.
                window.Hide();
            }
        }
    }
}