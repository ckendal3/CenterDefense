using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace WDIB.Components
{
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

    public struct Statistics : IComponentData
    {
        public float gameDuration; // how long the game has been going for
        public float enemyKills; // how many enemies have been killed
        public int projectilesShot; // how many projetiles have been shot
        public int upgradesBought; // how many upgrades have been bought
        public int currencyEarned; // how much currency has been earned
        public int damageDealt; // how much damage the player has given out
        public int damageTaken; // how much damage the player has taken
    }
}
