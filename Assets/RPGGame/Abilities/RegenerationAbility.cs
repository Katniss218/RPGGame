using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Abilities
{
    [RequireComponent( typeof( HealthHandler ) )]
    public class RegenerationAbility : MonoBehaviour
    {
        public enum RegenerationMode
        {
            Constant,
            Percent
        }

        /// <summary>
        /// The health points or float percent points (0-1) restored per second.
        /// </summary>
        public float HealthPerSecond = 1;

        public RegenerationMode Mode = RegenerationMode.Constant;

        /// <summary>
        /// The amount of time since last damage was taken before the regeneration begins.
        /// </summary>
        public float ActivationDelay = 0;

        private HealthHandler healthHandler;

        void Awake()
        {
            healthHandler = this.GetComponent<HealthHandler>();
        }

        void Update()
        {
            HandleRegeneration();
        }

        private void HandleRegeneration()
        {
            if( healthHandler.Health >= healthHandler.MaxHealth )
            {
                return;
            }
            if( healthHandler.TimeSinceLastDamage < ActivationDelay )
            {
                return;
            }

            float delta = 0;

            if( Mode == RegenerationMode.Constant )
            {
                delta = HealthPerSecond * Time.deltaTime;
            }

            if( Mode == RegenerationMode.Percent )
            {
                delta = (HealthPerSecond * healthHandler.MaxHealth) * Time.deltaTime;
            }

            // Cap the health so we don't overheal.
            float maxPossibleDelta = healthHandler.MaxHealth - healthHandler.Health;
            if( delta > maxPossibleDelta )
            {
                delta = maxPossibleDelta;
            }

            if( delta != 0 )
            {
                healthHandler.ChangeHealth( delta );
            }
        }
    }
}