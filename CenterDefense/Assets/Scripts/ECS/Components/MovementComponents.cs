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



    // previous position of an entity
    public struct PreviousTranslation : IComponentData
    {
        public float3 Value;

        // This allows us to do stuff like: previousTranslation - translation.value
        // it *implicitly* returns a float rather than the struct
        public static implicit operator float3(PreviousTranslation translation)
        {
            return translation.Value;
        }
    }

    // how fast an entity should move
    public struct Speed : IComponentData
    {
        public float Value;

        // This allows us to do stuff like: previousTranslation - translation.value
        // it *implicitly* returns a float rather than the struct
        public static implicit operator float(Speed speed)
        {
            return speed.Value;
        }
    }

    public struct Distance : IComponentData
    {
        public float distanceTravelled;
        public float maxAllowedDistance;
    }

}
