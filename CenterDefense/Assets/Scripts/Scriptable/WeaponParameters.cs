using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Weapon Parameters", menuName = "WDIB/Parameters/WeaponParameters")]
    public class WeaponParameters : SingletonScriptableObject<WeaponParameters>
    {
        [Header("Projectiles")]
        public List<ProjectileData> projectileSet;

        public float GlobalMaximumProjectileRange;
    }
}