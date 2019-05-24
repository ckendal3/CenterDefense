using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Turret Data", menuName = "WDIB/Data/Turret")]
    public class TurretData : ScriptableObject
    {
        [Tooltip("Name of the Turret")]
        public string turretName = "Default";

        [Tooltip("Max health of the Turret.")]
        [Range(1, 1000)]
        public float maxHealth = 200;

        [Tooltip("Cost to upgrade to this Turret upgrade.")]
        [Range(1, 1000)]
        public int upgradeCost = 10;

        [Header("Weapon")]
        [Tooltip("ID of the weapon to use when shooting")]
        public int weaponID;

        [Header("Visual Elements")]
        public Mesh mesh;
        public Material material;
    }
}