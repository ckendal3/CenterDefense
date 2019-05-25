using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WDIB.Parameters
{
    [CreateAssetMenu(fileName = "Stage Data", menuName = "WDIB/Data/Stage")]
    public class StageData : ScriptableObject
    {
        [Header("Player Spawn Settings")]
        public PlayerSpawnData playerSpawnData;

        [Header("Enemy Spawn Settings")]
        public EnemySpawnData enemySpawnData;

        [Header("Stage Multipliers")]
        public StageMultiplierData multiplierData;
    }
}

[System.Serializable]
public struct StageMultiplierData
{
    [Tooltip("The multiplier for killing an enemy.")]
    [Range(1, 10)]
    public float stageRewardMultiplier;

    [Tooltip("The multiplier increase of the enemies difficulty. Multiplies damage and health on enemies.")]
    [Range(1, 10)]
    public float enemyDifficultyMultiplier;

    [Tooltip("Multiplies the speed of enemies.")]
    [Range(1, 10)]
    public float enemySpeedMultiplier;
}