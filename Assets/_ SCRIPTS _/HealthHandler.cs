using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame
{
    /// <remarks>
    /// Attach to the root object.
    /// </remarks>
    public class HealthHandler : MonoBehaviour
    {
        public class OnHealthChangeEventInfo
        {
            public HealthHandler Self;
            public float Delta;
        }

        public class OnDeathEventInfo
        {
            public HealthHandler Self;
        }

        public float Health;
        public float MaxHealth;

        public UnityEvent<OnHealthChangeEventInfo> onHealthChange;
        public UnityEvent<OnDeathEventInfo> onDeath;

        public void TakeDamage( float amount )
        {
            Health -= amount;

            onHealthChange?.Invoke( new OnHealthChangeEventInfo()
            {
                Self = this,
                Delta = amount
            } );

            if( Health <= 0 )
            {
                Die();
            }
        }

        public void Die()
        {
            onDeath?.Invoke( new OnDeathEventInfo()
            {
                Self = this
            } );
        }
    }
}