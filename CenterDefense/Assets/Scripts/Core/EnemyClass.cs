using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyClass : MonoBehaviour
{
    [SerializeField]
    public EnemySpawnData spawnData;
}

[System.Serializable]
public struct EnemySpawnData
{
    [Tooltip("The total amount of enemies that will be spawned in this stage.")]
    public int maxEnemySpawnCount;

    [Tooltip("The amount of time in between enemies spawning.")]
    public float enemySpawnRate;

    [Tooltip("The amount of enemies that will spawn at a single time per spawn rate.")]
    public int enemiesPerASpawn;

    [Tooltip("The amount of enemies to be spawned at the beginning of the round.")]
    public int initialEnemySpawnAmount;

    [Tooltip("The chance of spawning a certain type of enemy.")]
    public EnemySpawnChance[] spawnChance;
}

public enum EEnemyType
{
    slow,
    normal,
    fast
}

[System.Serializable]
public struct EnemyData
{
    [Tooltip("The type of enemy to spawn.")]
    public EEnemyType type;
}

[System.Serializable]
public struct EnemySpawnChance
{
    public EEnemyType enemyType;

    [Tooltip("The percentage chance of an enemy type spawning. ")]
    [Range(0,1)]
    public float spawnChance;
}
