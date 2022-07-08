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
        public class HealthChangeEventInfo
        {
            public HealthHandler Self;
            public float Delta;
        }

        public class DeathEventInfo
        {
            public HealthHandler Self;
        }

        [SerializeField] private float _health;
        public float Health { get => _health; private set => _health = value; }

        [SerializeField] private float _maxHealth;
        public float MaxHealth { get => _maxHealth; private set => _maxHealth = value; }

        public UnityEvent<HealthChangeEventInfo> onMaxHealthChange;
        public UnityEvent<HealthChangeEventInfo> onHealthChange;
        public UnityEvent<DeathEventInfo> onDeath;

        /// <param name="amount">Amount can be negative, to heal.</param>
        public void TakeDamage( float amount )
        {
            Health -= amount;

            onHealthChange?.Invoke( new HealthChangeEventInfo()
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
            onDeath?.Invoke( new DeathEventInfo()
            {
                Self = this
            } );
        }
    }
}