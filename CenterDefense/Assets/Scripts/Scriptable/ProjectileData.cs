using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "WDIB/Projectiles")]
    public class ProjectileData : ScriptableObject
    {
        public string projectileName = "Default";

        // How many projectile to spawn per a shot
        [Range(1, 25)]
        public int projectilesPerAShot = 1;

        // How far before the projectile should destroy
        [Range(0f, 10000f)]
        public float maxDistance = 5000f;

        [Range(0f, 500f)]
        public float damage = 10f;

        // how many units should the projectile move a second
        [Range(0f, 1000f)]
        public float speed = 10f;

        // How long the projectile should last
        [Range(0f, 100f)]
        public float lifeTime = 8f;

        // Visual settings
        public Mesh mesh;
        public Material material;

        // Add ECS Component attaching?
    }
}

