using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public struct EnemySpawnSettings : IComponentData
{
    public int Value; // the reference for the enemy spawn settings to use
}

public struct CountTimeDown : IComponentData // count down from this value to zero
{
    public float Value; // time to countdown from -> goes to zero
}

public struct CountTimeUp : IComponentData // count up to this time
{
    public float Value; // current time
    public float maxDuration; // the duration to stop at when counting up
}

public struct enemyCount : IComponentData
{
    public int Value; // current count of enemies in the game
    public int maxCount; // maximum enemies spawned for the round
}

public struct RoundLimit : IComponentData
{
    public int Value; // current round
    public int maxCount; // max allowed rounds
}
