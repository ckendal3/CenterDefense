using Unity.Entities;
using Unity.Mathematics;

namespace WDIB.Components
{
    // This tag would work with any movement but especially with drones
    // Raycast to the ground and move up and down between a certain amount 
    // for a dynamic and alive feeling. A hover like feeling - not a perfect floating effect
    public struct SineMovementTag : ISharedComponentData { }

    // Distance value?
    // speed value?
    public struct SineMovementFloatiness : IComponentData
    {
        public float Value;
    }

    public struct PhysicsEntityTag : IComponentData { }
}
