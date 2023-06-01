using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPGGame.Items
{
    [CreateAssetMenu( fileName = "_weapon_item_", menuName = "Items/WeaponItem", order = 2 )]
    public class WeaponItem : UsableItem
    {
        public float MeleeDamage;
        public float MeleeRange;

        // later, ranged AND melee, or magic, etc items exist.
    }
}