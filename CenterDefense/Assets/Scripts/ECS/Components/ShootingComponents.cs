using Unity.Entities;
using Unity.Mathematics;

namespace WDIB.Components
{
    // This should just be an offset point?
    public struct ProjectileSpawnPoint : IComponentData
    {
        public float distance; // how far away to spawns
        public float3 heading;
        public float3 direction; // the direction
    }

    public struct ProjectileSpawnAmount : IComponentData
    {
        public int Value;
    }

    // Can this be more generic and be used like TimeBetweenActions?
    // This is how long something has to wait between shots - like between trigger pulls
    public struct TimeBetweenShots : IComponentData
    {
        public float resetValue; // resets to this value when timeLeft <= 0
        public float timeLeft; // this is the time before it can shoot again
    }

    public struct TimeBetweenBurstShots : IComponentData
    {
        public float resetValue; // resets to this value when timeLeft <= 0
        public float timeLeft; // this is the time before it can shoot again
    }


}
