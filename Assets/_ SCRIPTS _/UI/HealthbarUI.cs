using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPGGame.UI
{
    public class HealthbarUI : MonoBehaviour
    {
        float MaxValue;
        float Value;

        float percent => Value / MaxValue;

        [SerializeField] private Image bar;

        public void OnHealthOrMaxHealthChange( HealthHandler.HealthChangeEventInfo e )
        {
            this.Value = e.Self.Health;
            this.MaxValue = e.Self.MaxHealth;

            bar.fillAmount = percent;
        }

        public void OnDeath( HealthHandler.DeathEventInfo e )
        {
            Destroy( this.gameObject );
        }
    }
}