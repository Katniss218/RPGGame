using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_usable_item_", menuName = "Items/UsableItem", order = 2 )]
    public class UsableItem : Item
    {
        public class OnUseEventInfo
        {
            public UsableItem Self;
            public Transform User;
        }

        public UnityEvent<OnUseEventInfo> onUse;

        public float UseTime;

        public AudioClip UseSound;

        public virtual void Use(Transform user)
        {
            this.onUse?.Invoke( new OnUseEventInfo()
            {
                Self = this,
                User = user
            } );
        }
    }
}