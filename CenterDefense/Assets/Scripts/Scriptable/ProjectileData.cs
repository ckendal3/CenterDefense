using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Projectile", menuName = "WDIB/Projectiles")]
    public class ProjectileData : ScriptableObject
    {
        public string projectileName = "Default";

        // How many projectile to spawn per a shot
        public int projectilesPerAShot = 1;

        // How far before the projectile should destroy
        public float maxDistance = 5000f;

        public float damage = 10f;

        // how many units should the projectile move a second
        public float speed = 10f;

        // How long the projectile should last
        public float lifeTime = 8f;

        // Visual settings
        public Mesh mesh;
        public Material material;

        // Add ECS Component attaching?
    }
}

