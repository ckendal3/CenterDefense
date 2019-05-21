using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "WDIB/Weapon")]
    public class WeaponData : ScriptableObject
    {
        public string weaponName = "Default";

        public int projectileId = 0;

        public EFiringType firingType = EFiringType.Single;

        // used for single shot and automatic - trigger pull
        public float timeBetweenShots = .5f;

        // time between rounds in a burst to spawn
        public float timeBetweenBurstRounds = .2f;

        // Projectiles to spawn per burst
        public int burstAmount = 2;

        // Add ECS Component attaching?
        // headshot
        // explosive, etc
    }

    public enum EFiringType
    {
        Single,
        Burst,
        Automatic
    }

}