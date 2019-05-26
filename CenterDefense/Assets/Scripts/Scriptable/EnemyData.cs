using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Enemy Data", menuName = "WDIB/Data/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Tooltip("The name of the enemy type")]
        [SerializeField]
        public string enemyName = "Default Enemy";

        [Tooltip("The amount of damage on attack.")]
        [SerializeField]
        public EnemyStats stats;

        [Tooltip("Mesh to use")]
        public Mesh mesh;

        [Tooltip("Material to use")]
        public Material material;
    }

    [System.Serializable]
    public struct EnemySpawnData
    {
        [Tooltip("The total amount of enemies that will be spawned in this stage.")]
        [Range(1, 300)]
        public int maxEnemySpawnCount;

        [Tooltip("The amount of time in between enemies spawning.")]
        [Range(.1f, 5)]
        public float enemySpawnRate;

        [Tooltip("The amount of enemies that will spawn at a single time per spawn rate.")]
        [Range(1, 40)]
        public int enemiesPerASpawn;

        [Tooltip("The amount of enemies to be spawned at the beginning of the round.")]
        [Range(1, 40)]
        public int initialEnemySpawnAmount;

        [Tooltip("The chance of spawning a certain type of enemy.")]
        public EnemySpawnChance[] spawnChance;
    }

    [System.Serializable]
    public struct EnemyStats
    {
        [Tooltip("The amount of damage on attack.")]
        [Range(1, 200)]
        public float damage;

        [Tooltip("The time between attacks.")]
        [Range(.001f, 5f)]
        public float attackRate;

        [Tooltip("The enemies health.")]
        [Range(1, 500)]
        public float health;

        [Tooltip("How fast the enemy moves per a second.")]
        [Range(.1f, 20f)]
        public float speed;
    }

    public enum EEnemyType
    {
        slow,
        normal,
        fast
    }

    [System.Serializable]
    public struct EnemySpawnChance
    {
        public EEnemyType enemyType;

        [Tooltip("The percentage chance of an enemy type spawning. ")]
        [Range(0, 1)]
        public float spawnChance;

        public EnemyData enemyData;
    }
}