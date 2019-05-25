using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
    [SerializeField]
    public StageData stageData;
}

[System.Serializable]
public struct StageData
{
    [Header("Enemy Spawn Settings")]
    public EnemySpawnData enemySpawnData;

    [Header("Stage Multipliers")]
    public StageMultiplierData multiplierData;

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





