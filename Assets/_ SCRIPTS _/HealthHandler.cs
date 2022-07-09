using System;
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

        private float lastDamageTimestamp;
        public float TimeSinceLastDamage => Time.time - lastDamageTimestamp;

        private float lastHealTimestamp;
        public float TimeSinceLastHeal => Time.time - lastHealTimestamp;

        public void Update()
        {
            if( Input.GetKeyDown( KeyCode.C ) )
            {
                this.ChangeMaxHealth( 5 );
            }
        }

        public void SetHealth( float amount, float? maxAmount = null )
        {
            float delta = amount - Health;
            if( delta == 0 )
            {
                throw new ArgumentException( "Can't change health by '0'." );
            }

            if( maxAmount != null )
            {
                float maxDelta = maxAmount.Value - MaxHealth;
                if( maxDelta == 0 )
                {
                    throw new ArgumentException( "Can't change max health by '0'." );
                }

                ChangeMaxHealth( maxDelta );
            }

            ChangeHealth( delta );
        }

        public void ChangeMaxHealth( float delta )
        {
            if( delta == 0 )
            {
                throw new ArgumentException( "Can't change max health by '0'." );
            }

            MaxHealth += delta;
            onMaxHealthChange?.Invoke( new HealthChangeEventInfo()
            {
                Delta = delta,
                Self = this
            } );
        }

        public void ChangeHealth( float delta )
        {
            if( delta == 0 )
            {
                throw new ArgumentException( "Can't change health by '0'." );
            }

            Health += delta;
            if( delta < 0 )
            {
                lastDamageTimestamp = Time.time;
            }
            else if( delta > 0 )
            {
                lastHealTimestamp = Time.time;
            }

            onHealthChange?.Invoke( new HealthChangeEventInfo()
            {
                Self = this,
                Delta = delta
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