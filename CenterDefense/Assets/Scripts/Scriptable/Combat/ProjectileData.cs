using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Projectile Data", menuName = "WDIB/Data/Projectile")]
    public class ProjectileData : ScriptableObject
    {
        [Tooltip("Name of the Projectiles")]
        public string projectileName = "Default";

        // How many projectile to spawn per a shot
        [Range(1, 25)]
        public int projectilesPerAShot = 1;

        // How far before the projectile should destroy
        [Range(0f, 1000f)]
        public float maxDistance = 1000f;

        [Range(0f, 500f)]
        public float damage = 10f;

        // how many units should the projectile move a second
        [Range(0f, 1000f)]
        public float speed = 10f;

        // Visual settings
        public Mesh mesh;
        public Material material;

        // Add ECS Component attaching?
    }
}

public struct PelletSpread
{
    public float minimumSpread;
    public float maximumSpread;
}

