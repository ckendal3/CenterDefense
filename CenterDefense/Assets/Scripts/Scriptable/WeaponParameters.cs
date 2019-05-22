using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WDIB.Utilities;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Weapon Parameters", menuName = "WDIB/Parameters/WeaponParameters")]
    public class WeaponParameters : SingletonScriptableObject<WeaponParameters>
    {
        [Header("Weapons")]
        [SerializeField]
        private List<WeaponData> weaponSet;

        [Header("Projectiles")]
        [SerializeField]
        private List<ProjectileData> projectileSet;

        public float GlobalMaximumProjectileRange;

        public WeaponData GetWeaponData(int ID)
        {
            // if id is 1 and our array only has one we will miss on the array
            if (ID > weaponSet.Count)
            {
                Debug.LogError("Weapon ID doesn't exist - returning first projectile");
                return weaponSet[0];
            }
            else
            {
                return weaponSet[ID];
            }
        }

        public ProjectileData GetProjectileData(int ID)
        {
            // if id is 1 and our array only has one we will miss on the array
            if (ID > projectileSet.Count - 1)
            {
                Debug.LogError("Weapon ID doesn't exist - returning first projectile");
                return projectileSet[0];
            }
            else
            {
                return projectileSet[ID];
            }
        }
    }
}