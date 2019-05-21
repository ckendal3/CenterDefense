using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "WDIB/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "Default";

        public int projectileId = 0;

        public EFiringType firingType = EFiringType.single;

        // Add ECS Component attaching?
    }

    public enum EFiringType
    {
        single, 
        automatic,
        burst
    }

}