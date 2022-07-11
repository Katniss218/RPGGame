using RPGGame.Audio;
using RPGGame.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Globals
{
    /// <summary>
    /// An editor class used to hold scripts that can then be used as inputs for events for various item types.
    /// </summary>
    [CreateAssetMenu( fileName = "_item_script_", menuName = "Items/_SCRIPTS", order = 0 )]
    public class ItemScripts : ScriptableObject
    {
        public void SwingMeleeWeapon( UsableItem.OnUseEventInfo e )
        {
            WeaponItem weapon = e.Self as WeaponItem;
            if( weapon == null )
            {
                throw new InvalidOperationException( $"Event Handler was attached to an invalid object'." );
            }

            if( e.Self.UseSound != null )
            {
                AudioManager.PlaySound( e.Self.UseSound, e.User );
            }

            if( e.Target == null )
            {
                Collider[] collidersInRange = Physics.OverlapSphere( e.User.position, weapon.MeleeRange );

                foreach( var collider in collidersInRange )
                {
                    if( collider.transform == e.User )
                    {
                        continue;
                    }

                    HealthHandler hitHealthScript = collider.GetComponent<HealthHandler>();
                    if( hitHealthScript == null )
                    {
                        continue;
                    }

                    hitHealthScript.ChangeHealth( -weapon.MeleeDamage );
                }
            }
            else
            {
                if( e.Target == e.Self )
                {
                    return;
                }

                HealthHandler hitHealthScript = e.Target.GetComponent<HealthHandler>();
                if( hitHealthScript == null )
                {
                    return;
                }

                hitHealthScript.ChangeHealth( -weapon.MeleeDamage );
            }
        }
    }
}