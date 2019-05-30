using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Weapon Parameters", menuName = "WDIB/Parameters/WeaponParameters")]
    public class WeaponParameters : SingletonScriptableObject<WeaponParameters>
    {
        [Header("Global")]
        [Tooltip("This is the maximum distance any projectile should last.")]
        public float GlobalMaximumProjectileRange;

        [Header("Weapons")]
        public WeaponData[] weaponSet;

        [Header("Projectiles")]
        public ProjectileData[] projectileSet;

        [Header("Projectiles")]
        public ExplosiveData[] explosiveSet;
    }
}