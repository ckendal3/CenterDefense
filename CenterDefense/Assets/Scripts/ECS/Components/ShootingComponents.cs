using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace WDIB.Components
{
    // this entity has the ability to shoot
    public struct ShooterTag : IComponentData { }

    // This should just be an offset point?
    public struct ProjectileSpawnPoint : IComponentData
    {
        public float3 Value;
        //public float3 Heading;
    }

    public struct DoubleProjectileSpawnPoint : IComponentData
    {
        public float3 Position1;
        public float3 Position2;
    }

    public struct ProjectileSpawnAmount : IComponentData
    {
        public int Value;
    }

    // Can this be more generic and be used like TimeBetweenActions?
    // This is how long something has to wait between shots
    public struct TimeBetweenShots : IComponentData
    {
        public float Value; // this is the time it resets to every time it shoots
        public float timeLeft; // this is the time before it can shoot again
    }

}
